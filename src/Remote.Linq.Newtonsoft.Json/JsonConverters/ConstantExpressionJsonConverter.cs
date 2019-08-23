// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.JsonConverters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Reflection;

    internal class ConstantExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(ConstantExpression);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var target = new ConstantExpression();
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializer.ReferenceResolver.IsReferenced(null, value))
            {
                var reference = serializer.ReferenceResolver.GetReference(null, value);

                writer.WriteStartObject();
                writer.WritePropertyName("$ref");
                writer.WriteValue(reference);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteStartObject();

                var reference = serializer.ReferenceResolver.GetReference(null, value);
                writer.WritePropertyName("$id");
                writer.WriteValue(reference);

                writer.WritePropertyName("$type");
                var type = value.GetType().GetTypeInfo();
                var typeString = serializer.TypeNameAssemblyFormatHandling == TypeNameAssemblyFormatHandling.Full
                    ? type.AssemblyQualifiedName
                    : $"{type}, {type.Assembly.GetName().Name}";
                writer.WriteValue(typeString);

                var constantExpression = (ConstantExpression)value;

                writer.WritePropertyName(nameof(constantExpression.Type));
                serializer.Serialize(writer, constantExpression.Type);

                writer.WritePropertyName(nameof(constantExpression.Value));
                serializer.Serialize(writer, constantExpression.Value, typeof(object));

                writer.WriteEndObject();
            }
        }
    }
}
