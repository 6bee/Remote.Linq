// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Newtonsoft.Json.Converters
{
    using Aqua.Newtonsoft.Json.Converters;
    using Aqua.TypeSystem;
    using global::Newtonsoft.Json;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;

    public sealed class ExpressionConverter : ObjectConverter<Expression>
    {
        private const string ValueTypePropertyName = "ValueType";

        protected override void ReadObjectProperties(JsonReader reader, Expression result, Dictionary<string, Property> properties, JsonSerializer serializer)
        {
            if (result is ConstantExpression constantExpression)
            {
                reader.AssertProperty(nameof(ConstantExpression.Type));
                var typeInfo = reader.Read<TypeInfo>(serializer);
                constantExpression.Type = typeInfo;

                reader.Advance();
                if (reader.IsProperty(ValueTypePropertyName))
                {
                    typeInfo = reader.Read<TypeInfo>(serializer);
                    reader.Advance();
                }

                reader.AssertProperty(nameof(ConstantExpression.Value), false);
                var value = reader.Read(typeInfo, serializer);
                constantExpression.Value = value;

                reader.AssertEndObject();
            }
            else
            {
                base.ReadObjectProperties(reader, result, properties, serializer);
            }
        }

        protected override void WriteObjectProperties(JsonWriter writer, Expression instance, IReadOnlyCollection<Property> properties, JsonSerializer serializer)
        {
            if (instance is ConstantExpression constantExpression)
            {
                writer.WritePropertyName(nameof(ConstantExpression.Type));
                serializer.Serialize(writer, constantExpression.Type);

                var type = constantExpression.Type?.Type;
                if (constantExpression.Value != null)
                {
                    var valueType = constantExpression.Value.GetType();
                    if (valueType != type)
                    {
                        type = valueType;
                        writer.WritePropertyName(ValueTypePropertyName);
                        serializer.Serialize(writer, new TypeInfo(type, false, false));
                    }
                }

                writer.WritePropertyName(nameof(ConstantExpression.Value));
                serializer.Serialize(writer, constantExpression.Value, type);
            }
            else
            {
                base.WriteObjectProperties(writer, instance, properties, serializer);
            }
        }
    }
}
