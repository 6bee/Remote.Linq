namespace Remote.Linq
{
    using ProtoBuf.Meta;
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;

    public static class ProtoTypeModel
    {
        public static void Configure()
        {
            Configure(RuntimeTypeModel.Default);
        }

        public static void Configure(RuntimeTypeModel typeModel)
        {
            typeModel
                .ConfigureDynamicObject()
                .ConfigureExpression()
                .ConfigureConstantExpression()
                .ConfigureMemberBinding()
                .ConfigureMemberInfo()
                .ConfigureVariableQueryArgument();
        }
    }
}
