// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Text.Json;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Aqua.Dynamic;
using Aqua.EnumerableExtensions;
using Aqua.Text.Json;
using Aqua.Text.Json.Converters;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;
using Remote.Linq.SimpleQuery;
using Remote.Linq.Text.Json.Converters;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    /// Configures <see cref="JsonSerializerOptions"/> and adds <see cref="JsonConverter"/>s for <i>Aqua</i> and <i>Remote.Linq</i> types.
    /// </summary>
    /// <param name="options">Json serializer options to be ammended.</param>
    public static JsonSerializerOptions ConfigureRemoteLinq(this JsonSerializerOptions options)
        => options.ConfigureRemoteLinq(default(KnownTypesRegistry));

    /// <summary>
    /// Configures <see cref="JsonSerializerOptions"/> and adds <see cref="JsonConverter"/>s for <i>Aqua</i> and <i>Remote.Linq</i> types.
    /// </summary>
    /// <param name="options">Json serializer options to be ammended.</param>
    /// <param name="knownTypesRegistry">Type registry to control types for deserialization of <see cref="DynamicObject"/>s.</param>
    public static JsonSerializerOptions ConfigureRemoteLinq(this JsonSerializerOptions options, KnownTypesRegistry? knownTypesRegistry)
    {
        options.AssertNotNull();

        knownTypesRegistry ??= KnownTypesRegistry.Default;

        RegisterRemoteLinqKnownTypes(knownTypesRegistry);

        options.ConfigureAqua(knownTypesRegistry);

        if (!options.Converters.Any(static x => x.CanConvert(typeof(VariableQueryArgument))))
        {
            options.Converters.Add(new VariableQueryArgumentConverter(knownTypesRegistry));
        }

        if (!options.Converters.Any(static x => x.CanConvert(typeof(VariableQueryArgumentList))))
        {
            options.Converters.Add(new VariableQueryArgumentListConverter(knownTypesRegistry));
        }

        var remoteLinqAssembly = typeof(RemoteQueryable).Assembly;

        remoteLinqAssembly
            .GetTypes()
            .Where(static x => !x.IsAbstract)
            .Where(typeof(Expression).IsAssignableFrom)
            .RegisterJsonConverter(typeof(ExpressionConverter<>), options, knownTypesRegistry);

        // Workaround: there seems to be no proper way to deal with converters for abtract base types, hence we also register for abstract type.
        if (!options.Converters.Any(x => x.CanConvert(typeof(Expression))))
        {
            options.Converters.Add(new ExpressionConverter<Expression>(knownTypesRegistry, true));
        }

        remoteLinqAssembly
            .GetTypes()
            .Where(static x => x.IsClass && !x.IsAbstract && !x.IsGenericType)
            .Where(static x => x.GetCustomAttributes(typeof(DataContractAttribute), false).Length is not 0)
            .Where(static x
                => x.GetCustomAttribute<JsonConverterAttribute>(false)?.ConverterType is { } converterType
                && converterType.IsGenericType
                && converterType.GetGenericTypeDefinition() == typeof(ObjectConverter<>))
            .RegisterJsonConverter(typeof(ObjectConverter<>), options, knownTypesRegistry);

        if (!options.Converters.Any(static x => x.CanConvert(typeof(MemberBinding))))
        {
            options.Converters.Add(new ObjectConverter<MemberBinding>(knownTypesRegistry, true));
        }

        return options;
    }

    [SuppressMessage("Major Code Smell", "S1172:Unused method parameters should be removed", Justification = "False positive: 'knownTypesRegistry' used in local function")]
    private static void RegisterJsonConverter(this IEnumerable<Type> types, Type genericConverterType, JsonSerializerOptions options, KnownTypesRegistry knownTypesRegistry)
    {
        types
            .Where(x => !options.Converters.Any(c => c.CanConvert(x)))
            .Select(CreateJsonConverter)
            .ForEach(options.Converters.Add);

        JsonConverter CreateJsonConverter(Type type)
        {
            var converterType = genericConverterType.MakeGenericType(type);
            var converter = Activator.CreateInstance(converterType, knownTypesRegistry);
            return (JsonConverter)converter!;
        }
    }

    internal static KnownTypesRegistry RegisterRemoteLinqKnownTypes(this KnownTypesRegistry knownTypesRegistry)
    {
        var types = typeof(Expression).Assembly
            .GetExportedTypes()
            .Except([
                typeof(Query),
            ])
            .Where(static x => !x.IsGenericType)
            .Where(static x =>
            {
                var attributes = x.GetCustomAttributes(true);
                return attributes.Any(static a => a is SerializableAttribute)
                    || attributes.Any(static a => a is DataContractAttribute);
            });
        foreach (var type in types)
        {
            if (!knownTypesRegistry.TryRegister(type, type.Name))
            {
                throw new InvalidOperationException($"Failed to register '{type}' as known type.");
            }
        }

        return knownTypesRegistry;
    }
}