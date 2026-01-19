// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.Dynamic;
using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using System.Diagnostics.CodeAnalysis;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

partial class ExpressionTranslationExtensions
{
    private sealed class RemoteToSystemLinqTranslator
    {
        private sealed class ExpressionComparer : IEqualityComparer<RemoteLinq.ParameterExpression>, IEqualityComparer<RemoteLinq.LabelTarget>
        {
            internal static readonly ExpressionComparer Default = new();

            bool IEqualityComparer<RemoteLinq.ParameterExpression>.Equals(RemoteLinq.ParameterExpression? x, RemoteLinq.ParameterExpression? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.InstanceId == y.InstanceId;
            }

            int IEqualityComparer<RemoteLinq.ParameterExpression>.GetHashCode(RemoteLinq.ParameterExpression obj) => obj?.InstanceId ?? 0;

            bool IEqualityComparer<RemoteLinq.LabelTarget>.Equals(RemoteLinq.LabelTarget? x, RemoteLinq.LabelTarget? y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.InstanceId == y.InstanceId;
            }

            int IEqualityComparer<RemoteLinq.LabelTarget>.GetHashCode(RemoteLinq.LabelTarget obj) => obj?.InstanceId ?? 0;
        }

        private readonly Dictionary<RemoteLinq.ParameterExpression, SystemLinq.ParameterExpression> _parameterExpressionCache;
        private readonly Dictionary<RemoteLinq.LabelTarget, SystemLinq.LabelTarget> _labelTargetCache;
        private readonly ITypeResolver _typeResolver;
        private readonly IDynamicObjectMapper _dynamicObjectMapper;

        public RemoteToSystemLinqTranslator(IExpressionFromRemoteLinqContext expressionTranslatorContext)
        {
            expressionTranslatorContext.AssertNotNull();

            _parameterExpressionCache = new Dictionary<RemoteLinq.ParameterExpression, SystemLinq.ParameterExpression>(ExpressionComparer.Default);

            _labelTargetCache = new Dictionary<RemoteLinq.LabelTarget, SystemLinq.LabelTarget>(ExpressionComparer.Default);

            _typeResolver = expressionTranslatorContext.TypeResolver
                ?? throw new ArgumentException($"{nameof(expressionTranslatorContext.TypeResolver)} property must not be null.", nameof(expressionTranslatorContext));

            _dynamicObjectMapper = expressionTranslatorContext.ValueMapper
                ?? throw new ArgumentException($"{nameof(expressionTranslatorContext.ValueMapper)} property must not be null.", nameof(expressionTranslatorContext));
        }

        public SystemLinq.Expression ToExpression(RemoteLinq.Expression expression) => Visit(expression.CheckNotNull());

        [return: NotNullIfNotNull("node")]
        private SystemLinq.Expression? Visit(RemoteLinq.Expression? node)
            => node?.NodeType switch
            {
                null => null,
                RemoteLinq.ExpressionType.Binary => VisitBinary((RemoteLinq.BinaryExpression)node),
                RemoteLinq.ExpressionType.Block => VisitBlock((RemoteLinq.BlockExpression)node),
                RemoteLinq.ExpressionType.Call => VisitMethodCall((RemoteLinq.MethodCallExpression)node),
                RemoteLinq.ExpressionType.Conditional => VisitConditional((RemoteLinq.ConditionalExpression)node),
                RemoteLinq.ExpressionType.Constant => VisitConstant((RemoteLinq.ConstantExpression)node),
                RemoteLinq.ExpressionType.Default => VisitDefault((RemoteLinq.DefaultExpression)node),
                RemoteLinq.ExpressionType.Invoke => VisitInvoke((RemoteLinq.InvokeExpression)node),
                RemoteLinq.ExpressionType.Goto => VisitGoto((RemoteLinq.GotoExpression)node),
                RemoteLinq.ExpressionType.Label => VisitLabel((RemoteLinq.LabelExpression)node),
                RemoteLinq.ExpressionType.Lambda => VisitLambda((RemoteLinq.LambdaExpression)node),
                RemoteLinq.ExpressionType.ListInit => VisitListInit((RemoteLinq.ListInitExpression)node),
                RemoteLinq.ExpressionType.Loop => VisitLoop((RemoteLinq.LoopExpression)node),
                RemoteLinq.ExpressionType.MemberAccess => VisitMember((RemoteLinq.MemberExpression)node),
                RemoteLinq.ExpressionType.MemberInit => VisitMemberInit((RemoteLinq.MemberInitExpression)node),
                RemoteLinq.ExpressionType.New => VisitNew((RemoteLinq.NewExpression)node),
                RemoteLinq.ExpressionType.NewArray => VisitNewArray((RemoteLinq.NewArrayExpression)node),
                RemoteLinq.ExpressionType.Parameter => VisitParameter((RemoteLinq.ParameterExpression)node),
                RemoteLinq.ExpressionType.Switch => VisitSwitch((RemoteLinq.SwitchExpression)node),
                RemoteLinq.ExpressionType.Try => VisitTry((RemoteLinq.TryExpression)node),
                RemoteLinq.ExpressionType.TypeIs => VisitTypeIs((RemoteLinq.TypeBinaryExpression)node),
                RemoteLinq.ExpressionType.Unary => VisitUnary((RemoteLinq.UnaryExpression)node),
                _ => throw new NotSupportedException($"Unknown expression node type: '{node.NodeType}'"),
            };

        private System.Reflection.MethodInfo ResolveMethod(MethodInfo method)
            => method.ResolveMethod(_typeResolver)
            ?? throw new TypeResolverException($"Failed to resolve method '{method}'");

        private SystemLinq.Expression VisitSwitch(RemoteLinq.SwitchExpression node)
        {
            var defaultExpression = Visit(node.DefaultExpression);
            var switchValue = Visit(node.SwitchValue);
            var compareMethod = node.Comparison.ResolveMethod(_typeResolver);
            var cases = node.Cases?.Select(VisitSwitchCase);

            return SystemLinq.Expression.Switch(switchValue, defaultExpression, compareMethod, cases);
        }

        private SystemLinq.SwitchCase VisitSwitchCase(RemoteLinq.SwitchCase switchCase)
        {
            var body = Visit(switchCase.Body);
            var testCases = switchCase.TestValues.AsEmptyIfNull().Select(Visit);
            return SystemLinq.Expression.SwitchCase(body, testCases!);
        }

        private SystemLinq.Expression VisitTry(RemoteLinq.TryExpression node)
        {
            var body = Visit(node.Body);
            var type = node.Type.ResolveType(_typeResolver);
            var fault = node.Fault is null ? null : Visit(node.Fault);
            var @finally = node.Finally is null ? null : Visit(node.Finally);
            var handlers = node.Handlers.AsEmptyIfNull().Select(VisitCatchBlock);

            return SystemLinq.Expression.MakeTry(type, body, @finally, fault, handlers);
        }

        private SystemLinq.CatchBlock VisitCatchBlock(RemoteLinq.CatchBlock catchBlock)
        {
            var exceptionType = catchBlock.Test.ResolveType(_typeResolver);
            var exceptionParameter = catchBlock.Variable is null ? null : VisitParameter(catchBlock.Variable);
            var body = Visit(catchBlock.Body);
            var filter = catchBlock.Filter is null ? null : Visit(catchBlock.Filter);

            return SystemLinq.Expression.MakeCatchBlock(exceptionType, exceptionParameter, body, filter);
        }

        private SystemLinq.NewExpression VisitNew(RemoteLinq.NewExpression node)
        {
            if (node.Constructor is null)
            {
                var type = node.Type.ResolveType(_typeResolver);
                return SystemLinq.Expression.New(type);
            }

            var constructor = node.Constructor.ResolveConstructor(_typeResolver)
                ?? throw new RemoteLinqException($"Failed to resolve constructor of node {nameof(RemoteLinq.NewExpression)}");
            if (node.Arguments is null)
            {
                if (node.Members?.Count > 0)
                {
                    var members = node.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                    return SystemLinq.Expression.New(constructor, Array.Empty<SystemLinq.Expression>(), members);
                }
                else
                {
                    return SystemLinq.Expression.New(constructor);
                }
            }
            else
            {
                var arguments =
                    from a in node.Arguments
                    select Visit(a);
                if (node.Members?.Count > 0)
                {
                    var members = node.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                    return SystemLinq.Expression.New(constructor, arguments, members);
                }
                else
                {
                    return SystemLinq.Expression.New(constructor, arguments);
                }
            }
        }

        private SystemLinq.Expression VisitNewArray(RemoteLinq.NewArrayExpression node)
        {
            var expressions = VisitExpressionList(node.Expressions);
            var type = node.Type.ResolveType(_typeResolver);
            return node.NewArrayType switch
            {
                RemoteLinq.NewArrayType.NewArrayBounds => SystemLinq.Expression.NewArrayBounds(type, expressions),
                RemoteLinq.NewArrayType.NewArrayInit => SystemLinq.Expression.NewArrayInit(type, expressions),
                _ => throw new NotSupportedException($"Unhandled new array type {node.NewArrayType}"),
            };
        }

        private SystemLinq.Expression VisitMemberInit(RemoteLinq.MemberInitExpression node)
        {
            var n = VisitNew(node.NewExpression);
            var bindings = VisitBindingList(node.Bindings);
            return SystemLinq.Expression.MemberInit(n, bindings);
        }

        private SystemLinq.Expression VisitInvoke(RemoteLinq.InvokeExpression node)
        {
            var expression = Visit(node.Expression);
            var arguments =
                from i in node.Arguments ?? Enumerable.Empty<RemoteLinq.Expression>()
                select Visit(i);
            return SystemLinq.Expression.Invoke(expression, arguments);
        }

        private SystemLinq.Expression VisitBlock(RemoteLinq.BlockExpression node)
        {
            var type = node.Type.ResolveType(_typeResolver);
            var variables = node.Variables.AsEmptyIfNull().Select(VisitParameter);
            var expressions = node.Expressions.AsEmptyIfNull().Select(Visit);
            return type is null
                ? SystemLinq.Expression.Block(variables, expressions!)
                : SystemLinq.Expression.Block(type, variables, expressions!);
        }

        private IEnumerable<SystemLinq.MemberBinding> VisitBindingList(IEnumerable<RemoteLinq.MemberBinding> original)
        {
            var list =
                from i in original
                select VisitMemberBinding(i);
            return list.ToArray();
        }

        private SystemLinq.MemberBinding VisitMemberBinding(RemoteLinq.MemberBinding binding)
            => binding.BindingType switch
            {
                RemoteLinq.MemberBindingType.Assignment => VisitMemberAssignment((RemoteLinq.MemberAssignment)binding),
                RemoteLinq.MemberBindingType.MemberBinding => VisitMemberMemberBinding((RemoteLinq.MemberMemberBinding)binding),
                RemoteLinq.MemberBindingType.ListBinding => VisitMemberListBinding((RemoteLinq.MemberListBinding)binding),
                _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
            };

        private SystemLinq.MemberAssignment VisitMemberAssignment(RemoteLinq.MemberAssignment assignment)
        {
            var e = Visit(assignment.Expression);
            var m = assignment.Member.ResolveMemberInfo(_typeResolver);
            return SystemLinq.Expression.Bind(m, e);
        }

        private SystemLinq.MemberMemberBinding VisitMemberMemberBinding(RemoteLinq.MemberMemberBinding binding)
        {
            var bindings = VisitBindingList(binding.Bindings);
            var m = binding.Member.ResolveMemberInfo(_typeResolver);
            return SystemLinq.Expression.MemberBind(m, bindings);
        }

        private SystemLinq.MemberListBinding VisitMemberListBinding(RemoteLinq.MemberListBinding binding)
        {
            var initializers = VisitElementInitializerList(binding.Initializers);
            var m = binding.Member.ResolveMemberInfo(_typeResolver);
            return SystemLinq.Expression.ListBind(m, initializers);
        }

        private IEnumerable<SystemLinq.ElementInit> VisitElementInitializerList(IEnumerable<RemoteLinq.ElementInit> list)
            => list
            .Select(VisitElementInitializer)
            .ToArray();

        private SystemLinq.ElementInit VisitElementInitializer(RemoteLinq.ElementInit initializer)
        {
            var arguments = VisitExpressionList(initializer.Arguments);
            var m = initializer.AddMethod.ResolveMethod(_typeResolver)
                ?? throw new TypeResolverException($"Failed to resolve method '{initializer.AddMethod}'");
            return SystemLinq.Expression.ElementInit(m, arguments);
        }

        private IEnumerable<SystemLinq.Expression> VisitExpressionList(IEnumerable<RemoteLinq.Expression> list)
            => list
            .Select(x => Visit(x))
            .ToArray();

        private SystemLinq.Expression VisitListInit(RemoteLinq.ListInitExpression node)
        {
            var n = VisitNew(node.NewExpression);
            var initializers =
                from i in node.Initializers
                select SystemLinq.Expression.ElementInit(
                    ResolveMethod(i.AddMethod),
                    i.Arguments.Select(Visit)!);
            return SystemLinq.Expression.ListInit(n, initializers);
        }

        private SystemLinq.ParameterExpression VisitParameter(RemoteLinq.ParameterExpression node)
        {
            lock (_parameterExpressionCache)
            {
                if (!_parameterExpressionCache.TryGetValue(node, out var exp))
                {
                    var type = node.ParameterType.ResolveType(_typeResolver);
                    exp = SystemLinq.Expression.Parameter(type, node.ParameterName);
                    _parameterExpressionCache.Add(node, exp);
                }

                return exp;
            }
        }

        private SystemLinq.Expression VisitUnary(RemoteLinq.UnaryExpression node)
        {
            var expressionType = node.UnaryOperator.ToExpressionType();
            var exp = Visit(node.Operand);
            var type = node.Type.ResolveType(_typeResolver);
            var method = node.Method.ResolveMethod(_typeResolver);
            return SystemLinq.Expression.MakeUnary(expressionType, exp, type, method);
        }

        private SystemLinq.Expression VisitMember(RemoteLinq.MemberExpression node)
        {
            var exp = Visit(node.Expression);
            var m = node.Member.ResolveMemberInfo(_typeResolver);
            return SystemLinq.Expression.MakeMemberAccess(exp, m);
        }

        private SystemLinq.Expression VisitMethodCall(RemoteLinq.MethodCallExpression node)
        {
            var instance = Visit(node.Instance);
            var arguments = node.Arguments?
                .Select(x => Visit(x))
                .ToArray();
            var methodInfo = ResolveMethod(node.Method);
            return SystemLinq.Expression.Call(instance, methodInfo, arguments);
        }

        private SystemLinq.Expression VisitConditional(RemoteLinq.ConditionalExpression node)
        {
            var test = Visit(node.Test);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);

            if (ifFalse is SystemLinq.DefaultExpression && ifFalse.Type == typeof(void))
            {
                return SystemLinq.Expression.IfThen(test, ifTrue);
            }

            return SystemLinq.Expression.Condition(test, ifTrue, ifFalse);
        }

        private SystemLinq.Expression VisitConstant(RemoteLinq.ConstantExpression node)
        {
            var value = node.Value;
            var type = node.Type.ResolveType(_typeResolver);

            if (type is not null && typeof(SystemLinq.Expression).IsAssignableFrom(type) && node.Value is RemoteLinq.Expression expressionValue)
            {
                var expValue = Visit(expressionValue);
                value = expValue;
            }
            else if (type is not null && typeof(IEnumerable<SystemLinq.Expression>).IsAssignableFrom(type) && node.Value is IEnumerable<RemoteLinq.Expression> expressionCollection)
            {
                var list = VisitExpressionList(expressionCollection).ToArray();
                value = list;
            }
            else if (type == typeof(Type) && value is Aqua.TypeSystem.TypeInfo typeInfo)
            {
                value = typeInfo.ResolveType(_typeResolver);
            }
            else if (value is ConstantQueryArgument constantQueryArgument
                && constantQueryArgument.Value is DynamicObject dynamicObject
                && dynamicObject.Type is not null)
            {
                var copy = new DynamicObject(dynamicObject.Type);
                foreach (var property in dynamicObject.Properties.AsEmptyIfNull())
                {
                    var propertyValue = property.Value;
                    if (propertyValue is RemoteLinq.Expression expValue)
                    {
                        propertyValue = Visit(expValue);
                    }

                    copy.Add(property.Name, propertyValue);
                }

                value = _dynamicObjectMapper.Map(copy, type);
            }
            else if (value is string && type is not null && type != typeof(string))
            {
                var mapper = new DynamicObjectMapper();
                var obj = mapper.MapObject(value);
                value = mapper.Map(obj, type);
            }

            return type is null
                ? SystemLinq.Expression.Constant(value)
                : SystemLinq.Expression.Constant(value, type);
        }

        private SystemLinq.Expression VisitBinary(RemoteLinq.BinaryExpression node)
        {
            var p1 = Visit(node.LeftOperand);
            var p2 = Visit(node.RightOperand);
            var conversion = Visit(node.Conversion) as SystemLinq.LambdaExpression;
            var binaryType = node.BinaryOperator.ToExpressionType();
            var method = node.Method.ResolveMethod(_typeResolver);
            return SystemLinq.Expression.MakeBinary(binaryType, p1, p2, node.IsLiftedToNull, method, conversion);
        }

        private SystemLinq.Expression VisitTypeIs(RemoteLinq.TypeBinaryExpression node)
        {
            var expression = Visit(node.Expression);
            var type = node.TypeOperand.ResolveType(_typeResolver);
            return SystemLinq.Expression.TypeIs(expression, type);
        }

        private SystemLinq.Expression VisitLambda(RemoteLinq.LambdaExpression node)
        {
            var body = Visit(node.Expression);
            var parameters = node.Parameters?.Select(VisitParameter) ?? Enumerable.Empty<SystemLinq.ParameterExpression>();

            if (node.Type is null)
            {
                return SystemLinq.Expression.Lambda(body, parameters);
            }

            var delegateType = node.Type.ResolveType(_typeResolver);
            return SystemLinq.Expression.Lambda(delegateType, body, parameters);
        }

        private SystemLinq.Expression VisitDefault(RemoteLinq.DefaultExpression node)
        {
            var type = node.Type.ResolveType(_typeResolver);
            return SystemLinq.Expression.Default(type);
        }

        private SystemLinq.Expression VisitGoto(RemoteLinq.GotoExpression node)
        {
            var kind = node.Kind.ToGotoExpressionKind();
            var target = VisitTarget(node.Target);
            var value = Visit(node.Value);
            var type = node.Type.ResolveType(_typeResolver);
            return SystemLinq.Expression.MakeGoto(kind, target, value, type ?? target.Type);
        }

        private SystemLinq.Expression VisitLabel(RemoteLinq.LabelExpression node)
        {
            var target = VisitTarget(node.Target);
            var defaultValue = Visit(node.DefaultValue);
            return SystemLinq.Expression.Label(target, defaultValue);
        }

        private SystemLinq.Expression VisitLoop(RemoteLinq.LoopExpression node)
        {
            var body = Visit(node.Body);
            var breakLabel = VisitTarget(node.BreakLabel);
            var continueLabel = VisitTarget(node.ContinueLabel);
            return SystemLinq.Expression.Loop(body, breakLabel, continueLabel);
        }

        [return: NotNullIfNotNull("labelTarget")]
        private SystemLinq.LabelTarget? VisitTarget(RemoteLinq.LabelTarget? labelTarget)
        {
            if (labelTarget is null)
            {
                return null;
            }

            lock (_labelTargetCache)
            {
                if (!_labelTargetCache.TryGetValue(labelTarget, out var target))
                {
                    var targetType = labelTarget.Type.ResolveType(_typeResolver)
                        ?? throw new RemoteLinqException($"Failed to resolve label target type '{labelTarget.Type}'");
                    target = SystemLinq.Expression.Label(targetType, labelTarget.Name);
                    _labelTargetCache.Add(labelTarget, target);
                }

                return target;
            }
        }
    }
}