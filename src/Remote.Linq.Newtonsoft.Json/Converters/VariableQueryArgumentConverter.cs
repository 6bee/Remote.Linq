// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.Converters;

using Aqua.Newtonsoft.Json;
using Aqua.Newtonsoft.Json.Converters;
using Aqua.TypeSystem;
using global::Newtonsoft.Json;
using Remote.Linq.DynamicQuery;

public sealed class VariableQueryArgumentConverter : ObjectConverter<VariableQueryArgument>
{
    public VariableQueryArgumentConverter(KnownTypesRegistry knownTypeRegistry)
        : base(knownTypeRegistry)
    {
    }

    protected override void ReadObjectProperties(JsonReader reader, VariableQueryArgument result, Dictionary<string, Property> properties, JsonSerializer serializer)
    {
        reader.CheckNotNull().AssertProperty(nameof(VariableQueryArgument.Type));
        var typeInfo = reader.Read<TypeInfo>(serializer) ?? throw reader.CreateException($"{nameof(VariableQueryArgument.Type)} must not be null.");

        reader.AssertProperty(nameof(VariableQueryArgument.Value));
        var value = reader.Read(typeInfo, serializer);

        reader.AssertEndObject();

        result.CheckNotNull().Type = typeInfo;
        result.Value = value;
    }

    protected override void WriteObjectProperties(JsonWriter writer, VariableQueryArgument instance, IReadOnlyCollection<Property> properties, JsonSerializer serializer)
    {
        writer.CheckNotNull().WritePropertyName(nameof(VariableQueryArgument.Type));
        serializer.CheckNotNull().Serialize(writer, instance.CheckNotNull().Type);

        writer.WritePropertyName(nameof(VariableQueryArgument.Value));
        serializer.Serialize(writer, instance.Value, instance.Type?.ToType());
    }
}