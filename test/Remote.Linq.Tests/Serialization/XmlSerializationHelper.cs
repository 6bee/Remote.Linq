// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public static class XmlSerializationHelper
    {
        public static T Serialize<T>(this T graph)
            => Serialize(graph, null);

        public static T Serialize<T>(this T graph, Type[] extraTypes)
        {
            var serializer = new XmlSerializer(typeof(T), extraTypes);

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }

        public static T SerializeExpression<T>(T expression) where T : Remote.Linq.Expressions.Expression
            => SerializeExpression(expression, null);

        public static T SerializeExpression<T>(T expression, Type[] extraTypes) where T : Remote.Linq.Expressions.Expression
        {
            var exp1 = expression.ReplaceGenericQueryArgumentsByNonGenericArguments();
            var exp2 = Serialize(exp1, extraTypes);
            var exp3 = exp2.ReplaceNonGenericQueryArgumentsByGenericArguments();
            return exp3;
        }
    }
}
