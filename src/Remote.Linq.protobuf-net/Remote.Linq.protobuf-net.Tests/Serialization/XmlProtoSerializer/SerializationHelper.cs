// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using ProtoBuf.Meta;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System.IO;

    public static class SerializationHelper
    {
        private static readonly TypeModel _serializer;

        static SerializationHelper()
        {
            _serializer = TypeModel.Create()
                .ConfigureRemoteLinq()
                .Compile();
        }

        public static T Serialize<T>(this T graph)
        {
            return (T)_serializer.DeepClone(graph);
        }

        public static T SerializeExpression<T>(this T graph) where T : Expression
        {
            //return graph.Serialize();

            var graphToSerialize = graph.ReplaceGenericQueryArgumentsByNonGenericArguments();

            var deserializedGraph = graphToSerialize.Serialize();

            var graphToReturn = deserializedGraph.ReplaceNonGenericQueryArgumentsByGenericArguments();
            return graphToReturn;
        }
    }
}
