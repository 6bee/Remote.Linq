namespace Remote.Linq.Expressions
{
    using ProtoBuf.Meta;

    internal static class MemberBindingTypeModel
    {
        public static RuntimeTypeModel ConfigureMemberBinding(this RuntimeTypeModel typeModel)
        {
            var baseType = typeModel[typeof(MemberBinding)];
            var fieldNumber = 100;

            baseType.AddSubType(++fieldNumber, typeof(MemberAssignment));
            baseType.AddSubType(++fieldNumber, typeof(MemberListBinding));
            baseType.AddSubType(++fieldNumber, typeof(MemberMemberBinding));

            return typeModel;
        }
    }
}
