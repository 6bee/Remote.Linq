namespace Remote.Linq.Tests.Serialization
{
    using ProtoBuf;
    using Remote.Linq;
    using Remote.Linq.Expressions;

    public static class SerializationHelper
    {
        static SerializationHelper()
        {
            ProtoTypeModel.Configure();
        }

        public static T Serialize<T>(this T graph)
        {
            return Serializer.DeepClone<T>(graph);
        }

        public static T SerializeExpression<T>(this T graph) where T : Expression
        {
            return graph.Serialize();
        }
    }
}
