namespace Remote.Linq.Tests.Serialization
{
    using System.IO;
    using System.Runtime.Serialization;

    public static class SerializationHelper
    {
        public static T Serialize<T>(this T graph)
        {
            var serializer = new NetDataContractSerializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }


        public static T SerializeExpression<T>(this T graph) where T : Remote.Linq.Expressions.Expression
        {
            return graph.Serialize();
        }
    }
}
