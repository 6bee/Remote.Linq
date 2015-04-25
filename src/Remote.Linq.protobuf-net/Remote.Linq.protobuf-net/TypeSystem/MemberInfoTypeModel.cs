namespace Remote.Linq.TypeSystem
{
    using ProtoBuf.Meta;

    internal static class MemberInfoTypeModel
    {
        public static RuntimeTypeModel ConfigureMemberInfo(this RuntimeTypeModel typeModel)
        {
            var baseType = typeModel[typeof(MemberInfo)];
            var fieldNumber = 100;

            baseType.AddSubType(++fieldNumber, typeof(FieldInfo));
            baseType.AddSubType(++fieldNumber, typeof(PropertyInfo));
            baseType.AddSubType(++fieldNumber, typeof(MethodBaseInfo));

            var methodInfoType = typeModel[typeof(MethodBaseInfo)];
            methodInfoType.AddSubType(++fieldNumber, typeof(ConstructorInfo));
            methodInfoType.AddSubType(++fieldNumber, typeof(MethodInfo));

            return typeModel;
        }
    }
}
