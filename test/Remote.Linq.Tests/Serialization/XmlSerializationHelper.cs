// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public static class XmlSerializationHelper
    {
        public static T Clone<T>(this T graph) => Clone(graph, null);

        public static T Clone<T>(this T graph, Type[] extraTypes)
        {
            var serializer = new XmlSerializer(typeof(T), extraTypes);
            using var stream = new MemoryStream();
            serializer.Serialize(stream, graph);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)serializer.Deserialize(stream);
        }

        public static T CloneExpression<T>(T expression)
            where T : Remote.Linq.Expressions.Expression
            => CloneExpression(expression, null);

        public static T CloneExpression<T>(T expression, Type[] extraTypes)
            where T : Remote.Linq.Expressions.Expression
        {
            var exp1 = expression.ReplaceGenericQueryArgumentsByNonGenericArguments();
            var exp2 = Clone(exp1, extraTypes);
            var exp3 = exp2.ReplaceNonGenericQueryArgumentsByGenericArguments();
            return exp3;
        }
    }
}