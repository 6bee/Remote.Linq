// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Newtonsoft.Json;
    using Remote.Linq.ExpressionVisitors;
    using System;

    public static class JsonSerializationHelper
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented }.ConfigureRemoteLinq();

        public static T Serialize<T>(this T graph)
        {
            var json = JsonConvert.SerializeObject(graph, _serializerSettings);

            return JsonConvert.DeserializeObject<T>(json, _serializerSettings);
        }

        public static object Serialize(this object graph, Type type)
        {
            var json = JsonConvert.SerializeObject(graph, _serializerSettings);

            return JsonConvert.DeserializeObject(json, type, _serializerSettings);
        }

        public static T SerializeExpression<T>(T expression) where T : Remote.Linq.Expressions.Expression
        {
            var exp1 = expression.ReplaceGenericQueryArgumentsByNonGenericArguments();
            var exp2 = Serialize(exp1);
            var exp3 = exp2.ReplaceNonGenericQueryArgumentsByGenericArguments();
            return exp3;
        }
    }
}
