// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf;

using Remote.Linq.Protobuf.Mappers;
using System.ComponentModel;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class MapperResolverExtensions
{
    extension(MapperResolver resolver)
    {
        public static MapperResolver WithRemoteLinqTypes
            => new MapperResolver()
            .AddAquaTypes()
            .AddRemoteLinqTypes();

        public MapperResolver AddRemoteLinqTypes()
        {
            // query arguments
            resolver.AddMapper(ConstantQueryArgumentMapper.Instance);
            resolver.AddMapper(QueryableResourceDescriptorMapper.Instance);
            resolver.AddMapper(VariableQueryArgumentMapper.Instance);
            resolver.AddMapper(VariableQueryArgumentListMapper.Instance);
            resolver.AddMapper(SubstitutionValueMapper.Instance);

            // member bindings
            resolver.AddMapper(MemberBindingMapper.Instance);
            resolver.AddMapper(MemberAssignmentMapper.Instance);
            resolver.AddMapper(MemberListBindingMapper.Instance);
            resolver.AddMapper(ElementInitMapper.Instance);
            resolver.AddMapper(MemberMemberBindingMapper.Instance);

            // expressions
            resolver.AddMapper(ExpressionMapper.Instance);
            resolver.AddMapper(BinaryExpressionMapper.Instance);
            resolver.AddMapper(BlockExpressionMapper.Instance);
            resolver.AddMapper(ConditionalExpressionMapper.Instance);
            resolver.AddMapper(ConstantExpressionMapper.Instance);
            resolver.AddMapper(DefaultExpressionMapper.Instance);
            resolver.AddMapper(GotoExpressionMapper.Instance);
            resolver.AddMapper(LabelExpressionMapper.Instance);
            resolver.AddMapper(LabelTargetMapper.Instance);
            resolver.AddMapper(LambdaExpressionMapper.Instance);
            resolver.AddMapper(ListInitExpressionMapper.Instance);
            resolver.AddMapper(LoopExpressionMapper.Instance);
            resolver.AddMapper(MemberExpressionMapper.Instance);
            resolver.AddMapper(MemberInitExpressionMapper.Instance);
            resolver.AddMapper(MethodCallExpressionMapper.Instance);
            resolver.AddMapper(NewExpressionMapper.Instance);
            resolver.AddMapper(NewArrayExpressionMapper.Instance);
            resolver.AddMapper(ParameterExpressionMapper.Instance);
            resolver.AddMapper(SwitchExpressionMapper.Instance);
            resolver.AddMapper(SwitchCaseMapper.Instance);
            resolver.AddMapper(TryExpressionMapper.Instance);
            resolver.AddMapper(CatchBlockMapper.Instance);
            resolver.AddMapper(TypeBinaryExpressionMapper.Instance);
            resolver.AddMapper(UnaryExpressionMapper.Instance);

            return resolver;
        }
    }
}
