// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Newtonsoft.Json;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Aqua;
using Newtonsoft.Json.Serialization;
using Remote.Linq.Newtonsoft.Json.ContractResolvers;
using Remote.Linq.SimpleQuery;
using System.ComponentModel;
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
        settings.AssertNotNull();

        knownTypesRegistry ??= KnownTypesRegistry.Default;

        settings = settings.ConfigureAqua(knownTypesRegistry);

        knownTypesRegistry.RegisterKnownRemoteLinqTypes();

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

    extension(KnownTypesRegistry registry)
    {
        private void RegisterKnownRemoteLinqTypes()
        {
            var types = typeof(RemoteLinq.Expression).Assembly
                .GetExportedTypes()
                .Except(new[]
                {
                    typeof(Query),
                })
                .Where(static x => !x.IsGenericType)
                .Where(static x => x.GetCustomAttributes(true).Any(static a => a is SerializableAttribute or DataContractAttribute));
            foreach (var type in types)
            {
                if (!registry.TryRegister(type, type.Name))
                {
                    throw new InvalidOperationException($"Failed to register '{type}' as known type.");
                }
            }
        }
    }
}
