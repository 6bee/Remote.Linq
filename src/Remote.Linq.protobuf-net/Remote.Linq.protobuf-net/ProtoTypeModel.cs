namespace Remote.Linq
{
    using ProtoBuf.Meta;
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;

    public static class ProtoTypeModel
    {
        public static void ConfigureRemoteLinq()
        {
            ConfigureRemoteLinq(RuntimeTypeModel.Default);
        }

        public static RuntimeTypeModel ConfigureRemoteLinq(this RuntimeTypeModel typeModel)
        {
            return typeModel
                .ConfigureDynamicObject()
                .ConfigureExpression()
                .ConfigureConstantExpression()
                .ConfigureMemberBinding()
                .ConfigureMemberInfo()
                .ConfigureVariableQueryArgument();
        }
    }
}
