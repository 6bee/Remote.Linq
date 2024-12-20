// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Text.Json;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Aqua.Dynamic;
using Aqua.EnumerableExtensions;
using Aqua.Text.Json;
using Aqua.Text.Json.Converters;
using Remote.Linq.Expressions;
using Remote.Linq.SimpleQuery;
using Remote.Linq.Text.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

        RegisterKnownTypes(knownTypesRegistry);

        options.ConfigureAqua(knownTypesRegistry);

        if (!options.Converters.Any(x => x is VariableQueryArgumentConverter))
        {
            options.Converters.Add(new VariableQueryArgumentConverter(knownTypesRegistry));
        }

        if (!options.Converters.Any(x => x is VariableQueryArgumentListConverter))
        {
            options.Converters.Add(new VariableQueryArgumentListConverter(knownTypesRegistry));
        }

        // Workaround: there seems to be no proper way to deal with converters for abtract base types,
        // hence we register for abstract as well as non-abstract types.
        typeof(Expression).Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(typeof(Expression).IsAssignableFrom)
            .RegisterJsonConverter(typeof(ExpressionConverter<>), options, knownTypesRegistry);
        if (!options.Converters.Any(x => x is ExpressionConverter<Expression>))
        {
            options.Converters.Add(new ExpressionConverter<Expression>(knownTypesRegistry, true));
        }

        typeof(Expression).Assembly
            .GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType)
            .Where(x => x.GetCustomAttributes(typeof(DataContractAttribute), false).Length > 0)
            .RegisterJsonConverter(typeof(ObjectConverter<>), options, knownTypesRegistry);

        if (!options.Converters.Any(x => x is ObjectConverter<MemberBinding>))
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

    private static void RegisterKnownTypes(KnownTypesRegistry knownTypesRegistry)
    {
        var types = typeof(Expression).Assembly
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