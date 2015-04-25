namespace Remote.Linq
{
    using System.Linq;
    using ProtoBuf.Meta;

    internal static class VariableQueryArgumentTypeModel
    {
        public static RuntimeTypeModel ConfigureVariableQueryArgument(this RuntimeTypeModel typeModel)
        {
            var type = typeModel[typeof(VariableQueryArgument)];

            type.GetFields().Single(x => string.Equals(x.Name, "Value")).DynamicType = true;

            return typeModel;
        }
    }
}
