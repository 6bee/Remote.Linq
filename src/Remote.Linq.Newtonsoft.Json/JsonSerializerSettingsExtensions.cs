// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Newtonsoft.Json;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Aqua.Newtonsoft.Json;
using global::Newtonsoft.Json.Serialization;
using Remote.Linq.Newtonsoft.Json;
using Remote.Linq.Newtonsoft.Json.ContractResolvers;
using Remote.Linq.SimpleQuery;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using RemoteLinq = Remote.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class JsonSerializerSettingsExtensions
{
    /// <summary>
    /// Sets the <see cref="RemoteLinqContractResolver"/> in <see cref="JsonSerializerSettings"/>,
    /// decorating a previousely set <see cref="IContractResolver"/> if required.
    /// </summary>
    public static T ConfigureRemoteLinq<T>(this T settings, KnownTypesRegistry? knownTypesRegistry = null)
        where T : JsonSerializerSettings
    {
        knownTypesRegistry ??= new KnownTypesRegistry();

        settings = settings.CheckNotNull().ConfigureAqua(knownTypesRegistry);

        RegisterKnownTypes(knownTypesRegistry);

        if (settings.ContractResolver is not RemoteLinqContractResolver)
        {
            settings.ContractResolver = new RemoteLinqContractResolver(knownTypesRegistry, settings.ContractResolver);
        }

        return settings;
    }

    /// <summary>
    /// Create a new instance of <see cref="RemoteLinqJsonSerializerSettings"/>, based on the <see cref="JsonSerializerSettings"/> speficied.
    /// </summary>
    public static RemoteLinqJsonSerializerSettings CreateRemoteLinqConfiguration(this JsonSerializerSettings settings, KnownTypesRegistry? knownTypesRegistry = null)
    {
        var remoteLinqSettings = new RemoteLinqJsonSerializerSettings(settings, knownTypesRegistry);
        return remoteLinqSettings.ConfigureRemoteLinq(remoteLinqSettings.KnownTypesRegistry);
    }

    private static void RegisterKnownTypes(KnownTypesRegistry knownTypesRegistry)
    {
        var types = typeof(RemoteLinq.Expression).Assembly
            .GetExportedTypes()
            .Except(new[]
            {
                typeof(Query),
            })
            .Where(x => !x.IsGenericType)
            .Where(x =>
            {
                var attributes = x.GetCustomAttributes(true);
                return attributes.Any(a => a is SerializableAttribute)
                    || attributes.Any(a => a is DataContractAttribute);
            });
        foreach (var type in types)
        {
            if (!knownTypesRegistry.TryRegister(type, type.Name))
            {
                throw new InvalidOperationException($"Failed to register '{type}' as known type.");
            }
        }
    }
}