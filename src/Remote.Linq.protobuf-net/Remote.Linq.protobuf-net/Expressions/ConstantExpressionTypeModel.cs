namespace Remote.Linq.Expressions
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class ConstantExpressionTypeModel
    {
        public static RuntimeTypeModel ConfigureConstantExpression(this RuntimeTypeModel typeModel)
        {
            var type = typeModel[typeof(ConstantExpression)];

            type.GetFields().Single(x => string.Equals(x.Name, "Value")).DynamicType = true;

            return typeModel;
        }
    }
}
