// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Text.Json.Converters
{
    using Aqua.Text.Json;
    using Aqua.Text.Json.Converters;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.Text.Json;

    public class ExpressionConverter<TExpression> : ObjectConverter<TExpression>
        where TExpression : Expression
    {
        private const string ValueTypePropertyName = "ValueType";

        public ExpressionConverter(KnownTypesRegistry knownTypesRegistry)
            : base(knownTypesRegistry)
        {
        }

        protected override void ReadObjectProperties(ref Utf8JsonReader reader, TExpression result, Dictionary<string, Property> properties, JsonSerializerOptions options)
        {
            if (result is ConstantExpression constantExpression)
            {
                reader.AssertProperty(nameof(ConstantExpression.Type));
                var typeInfo = reader.Read<TypeInfo>(options);
                constantExpression.Type = typeInfo ?? throw reader.CreateException($"{nameof(ConstantExpression.Type)} must not be null.");

                reader.Advance();
                if (reader.IsProperty(ValueTypePropertyName))
                {
                    typeInfo = reader.Read<TypeInfo>(options);
                    reader.Advance();
                }

                reader.AssertProperty(nameof(ConstantExpression.Value), false);
                var value = reader.Read(typeInfo, options);
                constantExpression.Value = value;

                reader.AssertEndObject();
            }
            else
            {
                base.ReadObjectProperties(ref reader, result, properties, options);
            }
        }

        protected override void WriteObjectProperties(Utf8JsonWriter writer, TExpression instance, IReadOnlyCollection<Property> properties, JsonSerializerOptions options)
        {
            if (instance is ConstantExpression constantExpression)
            {
                writer.WritePropertyName(nameof(ConstantExpression.Type));
                writer.Serialize(constantExpression.Type, options);

                var type = constantExpression.Type?.ToType();
                if (constantExpression.Value is not null)
                {
                    var valueType = constantExpression.Value.GetType();
                    if (valueType != type)
                    {
                        type = valueType;
                        writer.WritePropertyName(ValueTypePropertyName);
                        writer.Serialize(type.AsTypeInfo(), options);
                    }
                }

                writer.WritePropertyName(nameof(ConstantExpression.Value));
                writer.Serialize(constantExpression.Value, options);
            }
            else
            {
                base.WriteObjectProperties(writer, instance, properties, options);
            }
        }
    }
}