namespace Remote.Linq.Tests.Serialization
{
    using Remote.Linq.ExpressionVisitors;
    using System.IO;
    using System.Runtime.Serialization;

    public static class SerializationHelper
    {
        public static T Serialize<T>(this T graph) where T : Remote.Linq.Expressions.Expression
        {
            var serializer = new DataContractSerializer(typeof(T));

            var graphToSerialize = graph.ReplaceGenericQueryArgumentsByNonGenericArguments();

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, graphToSerialize);

                stream.Seek(0, SeekOrigin.Begin);

                var deserializedGraph = (T)serializer.ReadObject(stream);

                var graphToReturn = deserializedGraph.ReplaceNonGenericQueryArgumentsByGenericArguments();
                return graphToReturn;
            }
        }
    }
}
