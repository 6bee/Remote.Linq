// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Text.Json.Converters
{
    using Aqua.Text.Json;
    using Aqua.Text.Json.Converters;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System.Collections.Generic;
    using System.Text.Json;

    public sealed class VariableQueryArgumentConverter : ObjectConverter<VariableQueryArgument>
    {
        public VariableQueryArgumentConverter(KnownTypesRegistry knownTypeRegistry)
            : base(knownTypeRegistry)
        {
        }

        protected override void ReadObjectProperties(ref Utf8JsonReader reader, VariableQueryArgument result, Dictionary<string, Property> properties, JsonSerializerOptions options)
        {
            reader.AssertProperty(nameof(VariableQueryArgument.Type));
            var typeInfo = reader.Read<TypeInfo>(options) ?? throw reader.CreateException($"{nameof(VariableQueryArgument.Type)} must not be null.");

            reader.AssertProperty(nameof(VariableQueryArgument.Value));
            var value = reader.Read(typeInfo, options);

            reader.AssertEndObject();

            result.CheckNotNull().Type = typeInfo;
            result.Value = value;
        }

        protected override void WriteObjectProperties(Utf8JsonWriter writer, VariableQueryArgument instance, IReadOnlyCollection<Property> properties, JsonSerializerOptions options)
        {
            writer.WritePropertyName(nameof(VariableQueryArgument.Type));
            writer.Serialize(instance.Type, options);

            writer.WritePropertyName(nameof(VariableQueryArgument.Value));
            writer.Serialize(instance.Value, options);
        }
    }
}