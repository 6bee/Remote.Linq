// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.MessagePack;

using Aqua.MessagePack;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Expressions;
using Remote.Linq.MessagePack.Formatters;

public sealed class RemoteLinqFormatterResolver(IFormatterResolver? fallback = null) : IFormatterResolver
{
    public static readonly RemoteLinqFormatterResolver Instance = new();

    private readonly IFormatterResolver _fallback = fallback ?? AquaFormatterResolver.Instance;

    private static readonly Dictionary<Type, object> _formatters = new()
    {
        [typeof(Expression)] = ExpressionFormatter.Instance,
        [typeof(BinaryExpression)] = BinaryExpressionFormatter.Instance,
        [typeof(BlockExpression)] = BlockExpressionFormatter.Instance,
        [typeof(ConditionalExpression)] = ConditionalExpressionFormatter.Instance,
        [typeof(ConstantExpression)] = ConstantExpressionFormatter.Instance,
        [typeof(DefaultExpression)] = DefaultExpressionFormatter.Instance,
        [typeof(GotoExpression)] = GotoExpressionFormatter.Instance,
        [typeof(InvokeExpression)] = InvokeExpressionFormatter.Instance,
        [typeof(LabelExpression)] = LabelExpressionFormatter.Instance,
        [typeof(LambdaExpression)] = LambdaExpressionFormatter.Instance,
        [typeof(ListInitExpression)] = ListInitExpressionFormatter.Instance,
        [typeof(LoopExpression)] = LoopExpressionFormatter.Instance,
        [typeof(MemberExpression)] = MemberExpressionFormatter.Instance,
        [typeof(MemberInitExpression)] = MemberInitExpressionFormatter.Instance,
        [typeof(MethodCallExpression)] = MethodCallExpressionFormatter.Instance,
        [typeof(NewExpression)] = NewExpressionFormatter.Instance,
        [typeof(NewArrayExpression)] = NewArrayExpressionFormatter.Instance,
        [typeof(ParameterExpression)] = ParameterExpressionFormatter.Instance,
        [typeof(SwitchExpression)] = SwitchExpressionFormatter.Instance,
        [typeof(TryExpression)] = TryExpressionFormatter.Instance,
        [typeof(TypeBinaryExpression)] = TypeBinaryExpressionFormatter.Instance,
        [typeof(UnaryExpression)] = UnaryExpressionFormatter.Instance,
        [typeof(CatchBlock)] = CatchBlockFormatter.Instance,
        [typeof(LabelTarget)] = LabelTargetFormatter.Instance,
        [typeof(SwitchCase)] = SwitchCaseFormatter.Instance,
        [typeof(ElementInit)] = ElementInitFormatter.Instance,
        [typeof(MemberBinding)] = MemberBindingFormatter.Instance,
        [typeof(MemberAssignment)] = MemberAssignmentFormatter.Instance,
        [typeof(MemberListBinding)] = MemberListBindingFormatter.Instance,
        [typeof(MemberMemberBinding)] = MemberMemberBindingFormatter.Instance,
        [typeof(ConstantQueryArgument)] = ConstantQueryArgumentFormatter.Instance,
        [typeof(VariableQueryArgument)] = VariableQueryArgumentFormatter.Instance,
        [typeof(VariableQueryArgumentList)] = VariableQueryArgumentListFormatter.Instance,
        [typeof(SubstitutionValue)] = SubstitutionValueFormatter.Instance,
        [typeof(QueryableResourceDescriptor)] = QueryableResourceDescriptorFormatter.Instance,
    };

    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        if (_formatters.TryGetValue(typeof(T), out var formatter))
        {
            return (IMessagePackFormatter<T>)formatter;
        }

        return _fallback.GetFormatter<T>();
    }
}
