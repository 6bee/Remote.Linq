// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.JsonConverters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Remote.Linq.Expressions;
    using System;

    public class ConstantExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ConstantExpression);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jToken = JValue.FromObject(value);
            serializer.Serialize(writer, jToken, typeof(ConstantExpression));
        }
    }
}
