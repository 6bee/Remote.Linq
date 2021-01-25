// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using Aqua.Utils;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionTranslator
    {
        private sealed class ResultWrapperExpression : SystemLinq.Expression
        {
            public ResultWrapperExpression(RemoteLinq.Expression result, Type type)
            {
                Result = result;
                Type = type;
            }

            public RemoteLinq.Expression Result { get; }

            public override Type Type { get; }

            public override bool CanReduce => false;

            public override SystemLinq.ExpressionType NodeType => (SystemLinq.ExpressionType)(-1);

            protected override SystemLinq.Expression Accept(SystemLinq.ExpressionVisitor visitor) => throw Exception;

            protected override SystemLinq.Expression VisitChildren(SystemLinq.ExpressionVisitor visitor) => throw Exception;

            public override SystemLinq.Expression Reduce() => throw Exception;

            private static Exception Exception => throw new RemoteLinqException($"{nameof(ResultWrapperExpression)} is meant for internal usage and must not be exposed externally.");
        }

        /// <summary>
        /// Combines two predicates with boolean AND. In case of one predicate is null, the other is returned without being combined.
        /// </summary>
        public static Func<T, bool>? And<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 is null)
            {
                return predicate2;
            }

            if (predicate2 is null)
            {
                return predicate1;
            }

            return x => predicate1(x) && predicate2(x);
        }

        /// <summary>
        /// Combines two predicates with boolean OR. In case of one predicate is null, the other is returned without being combined.
        /// </summary>
        public static Func<T, bool>? Or<T>(this Func<T, bool>? predicate1, Func<T, bool>? predicate2)
        {
            if (predicate1 is null)
            {
                return predicate2;
            }

            if (predicate2 is null)
            {
                return predicate1;
            }

            return x => predicate1(x) || predicate2(x);
        }

        /// <summary>
        /// Translates a given expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.Expression ToRemoteLinqExpression(this SystemLinq.Expression expression, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => new SystemLinqToRemoteLinqTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression.
        /// </summary>
        public static RemoteLinq.LambdaExpression ToRemoteLinqExpression(this SystemLinq.LambdaExpression expression, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var lambdaExpression = new SystemLinqToRemoteLinqTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);
            return (RemoteLinq.LambdaExpression)lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static SystemLinq.Expression ToLinqExpression(this RemoteLinq.Expression expression, ITypeResolver? typeResolver)
            => new RemoteLinqToSystemLinqTranslator(typeResolver).ToExpression(expression);

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.Expression<Func<T, TResult>> ToLinqExpression<T, TResult>(this RemoteLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = SystemLinq.Expression.Lambda<Func<T, TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.Expression<Func<TResult>> ToLinqExpression<TResult>(this RemoteLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = SystemLinq.Expression.Lambda<Func<TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static SystemLinq.LambdaExpression ToLinqExpression(this RemoteLinq.LambdaExpression expression, ITypeResolver? typeResolver)
            => (SystemLinq.LambdaExpression)new RemoteLinqToSystemLinqTranslator(typeResolver).ToExpression(expression);

        private static SystemLinq.ExpressionType ToExpressionType(this RemoteLinq.BinaryOperator binaryOperator)
            => (SystemLinq.ExpressionType)(int)binaryOperator;

        private static SystemLinq.ExpressionType ToExpressionType(this RemoteLinq.UnaryOperator unaryOperator)
            => (SystemLinq.ExpressionType)(int)unaryOperator;

        private static RemoteLinq.BinaryOperator ToBinaryOperator(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.BinaryOperator)(int)expressionType;

        private static RemoteLinq.UnaryOperator ToUnaryOperator(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.UnaryOperator)(int)expressionType;

        private static RemoteLinq.NewArrayType ToNewArrayType(this SystemLinq.ExpressionType expressionType)
            => (RemoteLinq.NewArrayType)(int)expressionType;

        private static SystemLinq.GotoExpressionKind ToGotoExpressionKind(this RemoteLinq.GotoExpressionKind kind)
            => (SystemLinq.GotoExpressionKind)(int)kind;

        private static RemoteLinq.GotoExpressionKind ToGotoExpressionKind(this SystemLinq.GotoExpressionKind kind)
            => (RemoteLinq.GotoExpressionKind)(int)kind;

        private static ResultWrapperExpression Wrap<T>(this T expression)
            where T : RemoteLinq.Expression
            => new ResultWrapperExpression(expression, typeof(T));

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. This method throws if expression is not an <see cref="SystemLinq.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static RemoteLinq.Expression Unwrap(this SystemLinq.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : throw new RemoteLinqException($"implementation error: expression is expected to be {nameof(SystemLinq.ConstantExpression)} but was {expression?.NodeType.ToString() ?? "<null>."}");

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. This method throws if expression is not an <see cref="SystemLinq.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static T Unwrap<T>(this SystemLinq.Expression? expression)
            where T : RemoteLinq.Expression
            => (T)Unwrap(expression);

        /// <summary>
        /// Unwraps the resulting <see cref="RemoteLinq.Expression"/>. The expression may be null.
        /// </summary>
        private static RemoteLinq.Expression? UnwrapNullable(this SystemLinq.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : null;

        private static bool KeepMarkerFunctions(SystemLinq.Expression expression)
        {
            if (expression is SystemLinq.MethodCallExpression methodCallExpression &&
                methodCallExpression.Method.IsGenericMethod &&
                methodCallExpression.Method.GetGenericMethodDefinition() == MethodInfos.QueryFuntion.Include)
            {
                return false;
            }

            return true;
        }

        private sealed class ConstantValueMapper : DynamicObjectMapper
        {
            private static readonly Func<Type, bool> _isPrimitiveType = new[]
                {
                    typeof(string),
                    typeof(int),
                    typeof(uint),
                    typeof(byte),
                    typeof(sbyte),
                    typeof(short),
                    typeof(ushort),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal),
                    typeof(char),
                    typeof(bool),
                    typeof(Guid),
                    typeof(DateTime),
                    typeof(TimeSpan),
                    typeof(DateTimeOffset),
                    typeof(System.Numerics.BigInteger),
                    typeof(System.Numerics.Complex),
                }
                .SelectMany(x => x.IsValueType ? new[] { x, typeof(Nullable<>).MakeGenericType(x) } : new[] { x })
                .ToDictionary(x => x, x => (object?)null)
                .ContainsKey;

            private static readonly Type[] _unmappedTypes = new[]
                {
                    typeof(CancellationToken),
                    typeof(ConstantQueryArgument),
                    typeof(VariableQueryArgument),
                    typeof(VariableQueryArgumentList),
                    typeof(QueryableResourceDescriptor),
                    typeof(VariableQueryArgument<>),
                    typeof(SystemLinq.Expression),
                    typeof(IQueryable),
                    typeof(IRemoteResource),
                };

            private static readonly Type[] _excludeFromUnmappedTypes = new[]
                {
                    typeof(EnumerableQuery),
                };

            private sealed class IsKnownTypeProvider : IIsKnownTypeProvider
            {
                private readonly bool _includePrimitiveType;

                public IsKnownTypeProvider(bool includePrimitiveType)
                {
                    _includePrimitiveType = includePrimitiveType;
                }

                public bool IsKnownType(Type type) => !TypeNeedsWrapping(type, _includePrimitiveType);
            }

            private ConstantValueMapper(ITypeResolver? typeResolver, ITypeInfoProvider? typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
                : base(typeResolver, typeInfoProvider, isKnownTypeProvider: isKnownTypeProvider)
            {
            }

            public static ConstantValueMapper ForSubstitution(ITypeInfoProvider typeInfoProvider)
                => new ConstantValueMapper(null, typeInfoProvider, new IsKnownTypeProvider(true));

            public static ConstantValueMapper ForReconstruction(ITypeResolver typeResolver)
                => new ConstantValueMapper(typeResolver, null, new IsKnownTypeProvider(false));

            public static bool TypeNeedsWrapping(Type type, bool includePrimitiveType = true)
            {
                if (includePrimitiveType && _isPrimitiveType(type))
                {
                    return false;
                }

                if (IsUnmappedType(type))
                {
                    return false;
                }

                return true;
            }

            private static bool IsUnmappedType(Type type)
            {
                var t = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                return _unmappedTypes.Any(x => x.IsAssignableFrom(t))
                    && !_excludeFromUnmappedTypes.Any(x => x.IsAssignableFrom(t));
            }
        }

        private sealed class SystemLinqToRemoteLinqTranslator : ExpressionVisitorBase
        {
            private readonly Dictionary<SystemLinq.ParameterExpression, RemoteLinq.ParameterExpression> _parameterExpressionCache =
                new Dictionary<SystemLinq.ParameterExpression, RemoteLinq.ParameterExpression>(ReferenceEqualityComparer<SystemLinq.ParameterExpression>.Default);

            private readonly Dictionary<SystemLinq.LabelTarget, RemoteLinq.LabelTarget> _labelTargetCache =
                new Dictionary<SystemLinq.LabelTarget, RemoteLinq.LabelTarget>(ReferenceEqualityComparer<SystemLinq.LabelTarget>.Default);

            private readonly Dictionary<object, ConstantQueryArgument> _constantQueryArgumentCache =
                new Dictionary<object, ConstantQueryArgument>(ReferenceEqualityComparer<object>.Default);

            private readonly Func<SystemLinq.Expression, bool>? _canBeEvaluatedLocally;
            private readonly ITypeInfoProvider _typeInfoProvider;

            public SystemLinqToRemoteLinqTranslator(ITypeInfoProvider? typeInfoProvider, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            {
                _canBeEvaluatedLocally = canBeEvaluatedLocally.And(KeepMarkerFunctions);
                _typeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            }

            public RemoteLinq.Expression ToRemoteExpression(SystemLinq.Expression expression)
            {
                var partialEvalExpression = expression.CheckNotNull(nameof(expression)).PartialEval(_canBeEvaluatedLocally);
                var constExpression = Visit(partialEvalExpression);
                return constExpression.Unwrap();
            }

            [return: NotNullIfNotNull("expression")]
            protected override SystemLinq.Expression? Visit(SystemLinq.Expression? node)
                => node?.NodeType switch
                {
                    SystemLinq.ExpressionType.New => VisitNew((SystemLinq.NewExpression)node).Wrap(),
                    _ => base.Visit(node),
                };

            protected override SystemLinq.Expression VisitSwitch(SystemLinq.SwitchExpression node)
            {
                var defaultExpression = Visit(node.DefaultBody).UnwrapNullable();
                var switchValue = Visit(node.SwitchValue).Unwrap();
                var cases = (node.Cases ?? Enumerable.Empty<SystemLinq.SwitchCase>()).Select(VisitSwitchCase).ToList();
                return new RemoteLinq.SwitchExpression(switchValue, node.Comparison, defaultExpression, cases).Wrap();
            }

            private new RemoteLinq.SwitchCase VisitSwitchCase(SystemLinq.SwitchCase switchCase)
            {
                var body = Visit(switchCase.Body).Unwrap();
                var testValues = switchCase.TestValues.Select(Visit).Select(Unwrap).ToList();
                return new RemoteLinq.SwitchCase(body, testValues);
            }

            protected override SystemLinq.Expression VisitTry(SystemLinq.TryExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var fault = Visit(node.Fault).UnwrapNullable();
                var @finally = Visit(node.Finally).UnwrapNullable();
                var handlers = node.Handlers?.Select(VisitCatch);
                return new RemoteLinq.TryExpression(_typeInfoProvider.GetTypeInfo(node.Type), body, fault, @finally, handlers).Wrap();
            }

            private new RemoteLinq.CatchBlock VisitCatch(SystemLinq.CatchBlock catchBlock)
            {
                var body = Visit(catchBlock.Body).Unwrap();
                var filter = Visit(catchBlock.Filter).UnwrapNullable();
                var variable = Visit(catchBlock.Variable).UnwrapNullable() as RemoteLinq.ParameterExpression;
                return new RemoteLinq.CatchBlock(_typeInfoProvider.GetTypeInfo(catchBlock.Test), variable, body, filter);
            }

            protected override SystemLinq.Expression VisitListInit(SystemLinq.ListInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var initializers = VisitElementInitializerList(node.Initializers);
                return new RemoteLinq.ListInitExpression(n, initializers).Wrap();
            }

            private new IEnumerable<RemoteLinq.ElementInit> VisitElementInitializerList(ReadOnlyCollection<SystemLinq.ElementInit> original)
                => original
                .Select(VisitElementInitializer)
                .ToArray();

            private new RemoteLinq.ElementInit VisitElementInitializer(SystemLinq.ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments).Select(Unwrap);
                return new RemoteLinq.ElementInit(_typeInfoProvider.GetMethodInfo(initializer.AddMethod), arguments);
            }

            private new RemoteLinq.NewExpression VisitNew(SystemLinq.NewExpression node)
            {
                IEnumerable<RemoteLinq.Expression>? arguments = null;
                if (node.Arguments?.Count > 0)
                {
                    arguments = node.Arguments
                        .Select(Visit)
                        .Select(Unwrap);
                }

                return node.Constructor is null
                    ? new RemoteLinq.NewExpression(_typeInfoProvider.GetTypeInfo(node.Type))
                    : new RemoteLinq.NewExpression(_typeInfoProvider.GetConstructorInfo(node.Constructor), arguments, node.Members?.Select(x => _typeInfoProvider.GetMemberInfo(x)));
            }

            protected override SystemLinq.Expression VisitConstant(SystemLinq.ConstantExpression node)
            {
                RemoteLinq.ConstantExpression exp;
                if (node.Type == typeof(Type) && node.Value is Type typeValue)
                {
                    exp = new RemoteLinq.ConstantExpression(typeValue.AsTypeInfo(), node.Type);
                }
                else if (node.Value is not null && ConstantValueMapper.TypeNeedsWrapping(node.Value.GetType()))
                {
                    var key = new { node.Value, node.Type };
                    if (!_constantQueryArgumentCache.TryGetValue(key, out var constantQueryArgument))
                    {
                        var dynamicObject = ConstantValueMapper.ForSubstitution(_typeInfoProvider).MapObject(node.Value);
                        constantQueryArgument = new ConstantQueryArgument(dynamicObject.Type);

                        _constantQueryArgumentCache.Add(key, constantQueryArgument);

                        foreach (var property in dynamicObject.Properties.AsEmptyIfNull())
                        {
                            var propertyValue = property.Value;
                            if (propertyValue is SystemLinq.Expression expressionValue)
                            {
                                propertyValue = Visit(expressionValue).UnwrapNullable();
                            }

                            constantQueryArgument.Add(property.Name, propertyValue);
                        }
                    }

                    exp = node.Type == constantQueryArgument.Type?.ToType()
                        ? new RemoteLinq.ConstantExpression(constantQueryArgument, constantQueryArgument.Type)
                        : new RemoteLinq.ConstantExpression(constantQueryArgument, _typeInfoProvider.GetTypeInfo(node.Type));
                }
                else
                {
                    exp = new RemoteLinq.ConstantExpression(node.Value, _typeInfoProvider.GetTypeInfo(node.Type));
                }

                return exp.Wrap();
            }

            protected override SystemLinq.Expression VisitParameter(SystemLinq.ParameterExpression node)
            {
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(node, out var exp))
                    {
                        exp = new RemoteLinq.ParameterExpression(_typeInfoProvider.GetTypeInfo(node.Type), node.Name, _parameterExpressionCache.Count + 1);
                        _parameterExpressionCache.Add(node, exp);
                    }

                    return exp.Wrap();
                }
            }

            protected override SystemLinq.Expression VisitBinary(SystemLinq.BinaryExpression node)
            {
                var binaryOperator = node.NodeType.ToBinaryOperator();
                var left = Visit(node.Left).Unwrap();
                var right = Visit(node.Right).Unwrap();
                var conversion = Visit(node.Conversion).UnwrapNullable() as RemoteLinq.LambdaExpression;
                return new RemoteLinq.BinaryExpression(binaryOperator, left, right, node.IsLiftedToNull, _typeInfoProvider.GetMethodInfo(node.Method), conversion).Wrap();
            }

            protected override SystemLinq.Expression VisitTypeIs(SystemLinq.TypeBinaryExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                return new RemoteLinq.TypeBinaryExpression(expression, _typeInfoProvider.GetTypeInfo(node.TypeOperand)).Wrap();
            }

            protected override SystemLinq.Expression VisitMemberAccess(SystemLinq.MemberExpression node)
            {
                var instance = Visit(node.Expression).UnwrapNullable();
                return new RemoteLinq.MemberExpression(instance, _typeInfoProvider.GetMemberInfo(node.Member)).Wrap();
            }

            protected override SystemLinq.Expression VisitMemberInit(SystemLinq.MemberInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var bindings = VisitBindingList(node.Bindings);
                return new RemoteLinq.MemberInitExpression(n, bindings).Wrap();
            }

            private new IEnumerable<RemoteLinq.MemberBinding> VisitBindingList(ReadOnlyCollection<SystemLinq.MemberBinding> original)
                => original
                .Select(x => VisitMemberBinding(x))
                .ToArray();

            private new RemoteLinq.MemberBinding VisitMemberBinding(SystemLinq.MemberBinding binding)
                => binding.BindingType switch
                {
                    SystemLinq.MemberBindingType.Assignment => VisitMemberAssignment((SystemLinq.MemberAssignment)binding),
                    SystemLinq.MemberBindingType.MemberBinding => VisitMemberMemberBinding((SystemLinq.MemberMemberBinding)binding),
                    SystemLinq.MemberBindingType.ListBinding => VisitMemberListBinding((SystemLinq.MemberListBinding)binding),
                    _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
                };

            private new RemoteLinq.MemberAssignment VisitMemberAssignment(SystemLinq.MemberAssignment assignment)
            {
                var expression = Visit(assignment.Expression).Unwrap();
                var member = _typeInfoProvider.GetMemberInfo(assignment.Member);
                return new RemoteLinq.MemberAssignment(member, expression);
            }

            private new RemoteLinq.MemberMemberBinding VisitMemberMemberBinding(SystemLinq.MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RemoteLinq.MemberMemberBinding(m, bindings);
            }

            private new RemoteLinq.MemberListBinding VisitMemberListBinding(SystemLinq.MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RemoteLinq.MemberListBinding(m, initializers);
            }

            protected override SystemLinq.Expression VisitMethodCall(SystemLinq.MethodCallExpression node)
            {
                var instance = Visit(node.Object).UnwrapNullable();
                var arguments = node.Arguments
                    .Select(Visit)
                    .Select(Unwrap);
                return new RemoteLinq.MethodCallExpression(instance, _typeInfoProvider.GetMethodInfo(node.Method), arguments).Wrap();
            }

            protected override SystemLinq.Expression VisitLambda(SystemLinq.LambdaExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var parameters = node.Parameters
                    .Select(VisitParameter)
                    .Select(Unwrap<RemoteLinq.ParameterExpression>);
                return new RemoteLinq.LambdaExpression(_typeInfoProvider.GetTypeInfo(node.Type), body, parameters).Wrap();
            }

            protected override SystemLinq.Expression VisitUnary(SystemLinq.UnaryExpression node)
            {
                var unaryOperator = node.NodeType.ToUnaryOperator();
                var operand = Visit(node.Operand).Unwrap();
                return new RemoteLinq.UnaryExpression(unaryOperator, operand, _typeInfoProvider.GetTypeInfo(node.Type), _typeInfoProvider.GetMethodInfo(node.Method)).Wrap();
            }

            protected override SystemLinq.Expression VisitConditional(SystemLinq.ConditionalExpression node)
            {
                var test = Visit(node.Test).Unwrap();
                var ifTrue = Visit(node.IfTrue).Unwrap();
                var ifFalse = Visit(node.IfFalse).Unwrap();
                return new RemoteLinq.ConditionalExpression(test, ifTrue, ifFalse).Wrap();
            }

            protected override SystemLinq.Expression VisitNewArray(SystemLinq.NewArrayExpression node)
            {
                var newArrayType = node.NodeType.ToNewArrayType();
                var expressions = VisitExpressionList(node.Expressions).Select(Unwrap);
                var elementType = TypeHelper.GetElementType(node.Type) ?? throw new RemoteLinqException($"Failed to get element type of {node.Type}.");
                return new RemoteLinq.NewArrayExpression(newArrayType, _typeInfoProvider.GetTypeInfo(elementType), expressions).Wrap();
            }

            protected override SystemLinq.Expression VisitInvocation(SystemLinq.InvocationExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                var arguments = VisitExpressionList(node.Arguments)?.Select(Unwrap);
                return new RemoteLinq.InvokeExpression(expression, arguments).Wrap();
            }

            protected override SystemLinq.Expression VisitBlock(SystemLinq.BlockExpression node)
            {
                var expressions = VisitExpressionList(node.Expressions)?.Select(Unwrap);
                IEnumerable<RemoteLinq.ParameterExpression>? variables = null;
                if (node.Variables is not null)
                {
                    var nodeVariables = node.Variables.Cast<SystemLinq.Expression>().ToList().AsReadOnly();
                    variables = VisitExpressionList(nodeVariables)?.Select(Unwrap<RemoteLinq.ParameterExpression>);
                }

                var type = node.Type == node.Result.Type ? null : node.Type;
                return new RemoteLinq.BlockExpression(_typeInfoProvider.GetTypeInfo(type), variables, expressions).Wrap();
            }

            protected override SystemLinq.Expression VisitDefault(SystemLinq.DefaultExpression node)
            {
                return new RemoteLinq.DefaultExpression(_typeInfoProvider.GetTypeInfo(node.Type)).Wrap();
            }

            protected override SystemLinq.Expression VisitLabel(SystemLinq.LabelExpression node)
            {
                var target = VisitTarget(node.Target);
                var defaultValue = Visit(node.DefaultValue).UnwrapNullable();
                return new RemoteLinq.LabelExpression(target, defaultValue).Wrap();
            }

            protected override SystemLinq.Expression VisitLoop(SystemLinq.LoopExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var breakLabel = VisitTarget(node.BreakLabel);
                var continueLabel = VisitTarget(node.ContinueLabel);
                return new RemoteLinq.LoopExpression(body, breakLabel, continueLabel).Wrap();
            }

            protected override SystemLinq.Expression VisitGoto(SystemLinq.GotoExpression node)
            {
                var kind = node.Kind.ToGotoExpressionKind();
                var target = VisitTarget(node.Target);
                var type = node.Target.Type == node.Type ? null : node.Type;
                var value = Visit(node.Value).UnwrapNullable();
                return new RemoteLinq.GotoExpression(kind, target, _typeInfoProvider.GetTypeInfo(type), value).Wrap();
            }

            [return: NotNullIfNotNull("labelTarget")]
            private RemoteLinq.LabelTarget? VisitTarget(SystemLinq.LabelTarget? labelTarget)
            {
                if (labelTarget is null)
                {
                    return null;
                }

                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out var target))
                    {
                        target = new RemoteLinq.LabelTarget(labelTarget.Name, _typeInfoProvider.GetTypeInfo(labelTarget.Type), _labelTargetCache.Count + 1);
                        _labelTargetCache.Add(labelTarget, target);
                    }

                    return target;
                }
            }
        }

        private sealed class RemoteLinqToSystemLinqTranslator : IEqualityComparer<RemoteLinq.ParameterExpression>, IEqualityComparer<RemoteLinq.LabelTarget>
        {
            private readonly Dictionary<RemoteLinq.ParameterExpression, SystemLinq.ParameterExpression> _parameterExpressionCache;
            private readonly Dictionary<RemoteLinq.LabelTarget, SystemLinq.LabelTarget> _labelTargetCache;
            private readonly ITypeResolver _typeResolver;

            public RemoteLinqToSystemLinqTranslator(ITypeResolver? typeResolver)
            {
                _parameterExpressionCache = new Dictionary<RemoteLinq.ParameterExpression, SystemLinq.ParameterExpression>(this);
                _labelTargetCache = new Dictionary<RemoteLinq.LabelTarget, SystemLinq.LabelTarget>(this);
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            public SystemLinq.Expression ToExpression(RemoteLinq.Expression expression) => Visit(expression.CheckNotNull(nameof(expression)));

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
                    _ => throw new NotSupportedException($"Unknown expression note type: '{node.NodeType}'"),
                };

            private System.Reflection.MethodInfo ResolveMethod(Aqua.TypeSystem.MethodInfo method)
                => method.ResolveMethod(_typeResolver)
                ?? throw new RemoteLinqException($"Failed to resolve method '{method}'");

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

                var constructor = node.Constructor.ResolveConstructor(_typeResolver) !;
                if (node.Arguments is null)
                {
                    if (node.Members?.Any() ?? false)
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
                    if (node.Members?.Any() ?? false)
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
                var m = initializer.AddMethod.ResolveMethod(_typeResolver) ?? throw new RemoteLinqException($"Failed to resolve method '{initializer.AddMethod}'");
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
                        i.Arguments.Select(Visit) !);
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

                if (type == typeof(Type) && value is Aqua.TypeSystem.TypeInfo typeInfo)
                {
                    value = typeInfo.ResolveType(_typeResolver);
                }
                else if (value is ConstantQueryArgument oldConstantQueryArgument && oldConstantQueryArgument.Type is not null)
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(oldConstantQueryArgument.Type);
                    foreach (var property in oldConstantQueryArgument.Properties.AsEmptyIfNull())
                    {
                        var propertyValue = property.Value;
                        if (propertyValue is RemoteLinq.Expression expressionValue)
                        {
                            propertyValue = Visit(expressionValue);
                        }

                        newConstantQueryArgument.Add(property.Name, propertyValue);
                    }

                    value = ConstantValueMapper.ForReconstruction(_typeResolver).Map(newConstantQueryArgument, type);
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
                        var targetType = labelTarget.Type.ResolveType(_typeResolver) ?? throw new RemoteLinqException($"Failed to resolve label target type '{labelTarget.Type}'");
                        target = SystemLinq.Expression.Label(targetType, labelTarget.Name);
                        _labelTargetCache.Add(labelTarget, target);
                    }

                    return target;
                }
            }

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
    }
}
