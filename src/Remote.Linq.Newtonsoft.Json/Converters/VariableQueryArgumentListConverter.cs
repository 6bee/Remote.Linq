// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.Converters
{
    using Aqua.Newtonsoft.Json.Converters;
    using Aqua.TypeSystem;
    using global::Newtonsoft.Json;
    using Remote.Linq.DynamicQuery;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class VariableQueryArgumentListConverter : ObjectConverter<VariableQueryArgumentList>
    {
        protected override void ReadObjectProperties(JsonReader reader, VariableQueryArgumentList result, Dictionary<string, Property> properties, JsonSerializer serializer)
        {
            TypeInfo elementTypeInfo;
            void SetResult(List<object> values = null)
            {
                reader.AssertEndObject();
                result.ElementType = elementTypeInfo;
                result.Values = values?.ToList();
            }

            reader.AssertProperty(nameof(VariableQueryArgumentList.ElementType));
            elementTypeInfo = reader.Read<TypeInfo>(serializer);

            reader.Advance();
            if (reader.TokenType == JsonToken.Null)
            {
                SetResult();
                return;
            }

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw reader.CreateException($"Expected array");
            }

            var elementType = elementTypeInfo.Type;
            var values = new List<object>();
            while (true)
            {
                if (!reader.TryRead(elementType, serializer, out object value))
                {
                    // TODO: is max length quota required?
                    if (reader.TokenType == JsonToken.EndArray)
                    {
                        break;
                    }

                    throw reader.CreateException("Unexpected token structure.");
                }

                values.Add(value);
            }

            SetResult(values);
        }

        protected override void WriteObjectProperties(JsonWriter writer, VariableQueryArgumentList instance, IReadOnlyCollection<Property> properties, JsonSerializer serializer)
        {
            writer.WritePropertyName(nameof(VariableQueryArgumentList.ElementType));
            serializer.Serialize(writer, instance.ElementType);

            writer.WritePropertyName(nameof(VariableQueryArgumentList.Values));
            serializer.Serialize(writer, instance.Values, instance.ElementType?.Type);
        }
    }
}
