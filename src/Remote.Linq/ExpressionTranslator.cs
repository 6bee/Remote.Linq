// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.Extensions;
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
    using RLinq = Remote.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionTranslator
    {
        private sealed class ResultWrapperExpression : System.Linq.Expressions.Expression
        {
            public ResultWrapperExpression(RLinq.Expression result, Type type)
            {
                Result = result;
                Type = type;
            }

            public RLinq.Expression Result { get; }

            public override Type Type { get; }

            public override bool CanReduce => false;

            public override System.Linq.Expressions.ExpressionType NodeType => (System.Linq.Expressions.ExpressionType)(-1);

            protected override System.Linq.Expressions.Expression Accept(System.Linq.Expressions.ExpressionVisitor visitor) => throw Exception;

            protected override System.Linq.Expressions.Expression VisitChildren(System.Linq.Expressions.ExpressionVisitor visitor) => throw Exception;

            public override System.Linq.Expressions.Expression Reduce() => throw Exception;

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
        public static RLinq.Expression ToRemoteLinqExpression(this System.Linq.Expressions.Expression expression, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new LinqExpressionToRemoteExpressionTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression.
        /// </summary>
        public static RLinq.LambdaExpression ToRemoteLinqExpression(this System.Linq.Expressions.LambdaExpression expression, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var lambdaExpression = new LinqExpressionToRemoteExpressionTranslator(typeInfoProvider, canBeEvaluatedLocally).ToRemoteExpression(expression);
            return (RLinq.LambdaExpression)lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static System.Linq.Expressions.Expression ToLinqExpression(this RLinq.Expression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into an expression.
        /// </summary>
        public static System.Linq.Expressions.Expression ToLinqExpression(this RLinq.Expression expression, ITypeResolver? typeResolver)
            => new RemoteExpressionToLinqExpressionTranslator(typeResolver).ToExpression(expression);

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static System.Linq.Expressions.Expression<Func<T, TResult>> ToLinqExpression<T, TResult>(this RLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = System.Linq.Expressions.Expression.Lambda<Func<T, TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static System.Linq.Expressions.Expression<Func<TResult>> ToLinqExpression<TResult>(this RLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = System.Linq.Expressions.Expression.Lambda<Func<TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static System.Linq.Expressions.LambdaExpression ToLinqExpression(this RLinq.LambdaExpression expression)
            => ToLinqExpression(expression, null);

        /// <summary>
        /// Translates a given query expression into a lambda expression.
        /// </summary>
        public static System.Linq.Expressions.LambdaExpression ToLinqExpression(this RLinq.LambdaExpression expression, ITypeResolver? typeResolver)
            => (System.Linq.Expressions.LambdaExpression)new RemoteExpressionToLinqExpressionTranslator(typeResolver).ToExpression(expression);

        private static System.Linq.Expressions.ExpressionType ToExpressionType(this RLinq.BinaryOperator binaryOperator)
            => (System.Linq.Expressions.ExpressionType)(int)binaryOperator;

        private static System.Linq.Expressions.ExpressionType ToExpressionType(this RLinq.UnaryOperator unaryOperator)
            => (System.Linq.Expressions.ExpressionType)(int)unaryOperator;

        private static System.Linq.Expressions.GotoExpressionKind ToGotoExpressionKind(this RLinq.GotoExpressionKind kind)
            => (System.Linq.Expressions.GotoExpressionKind)(int)kind;

        private static RLinq.BinaryOperator ToBinaryOperator(this System.Linq.Expressions.ExpressionType expressionType)
            => (RLinq.BinaryOperator)(int)expressionType;

        private static RLinq.UnaryOperator ToUnaryOperator(this System.Linq.Expressions.ExpressionType expressionType)
            => (RLinq.UnaryOperator)(int)expressionType;

        private static RLinq.NewArrayType ToNewArrayType(this System.Linq.Expressions.ExpressionType expressionType)
            => (RLinq.NewArrayType)(int)expressionType;

        private static RLinq.GotoExpressionKind ToGotoExpressionKind(this System.Linq.Expressions.GotoExpressionKind kind)
            => (RLinq.GotoExpressionKind)(int)kind;

        private static ResultWrapperExpression Wrap<T>(this T expression)
            where T : RLinq.Expression
            => new ResultWrapperExpression(expression, typeof(T));

        /// <summary>
        /// Unwraps the resulting <see cref="RLinq.Expression"/>. This method throws if expression is not an <see cref="System.Linq.Expressions.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static RLinq.Expression Unwrap(this System.Linq.Expressions.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : throw new RemoteLinqException($"implementation error: expression is expected to be {nameof(System.Linq.Expressions.ConstantExpression)} but was {expression?.NodeType.ToString() ?? "<null>."}");

        /// <summary>
        /// Unwraps the resulting <see cref="RLinq.Expression"/>. This method throws if expression is not an <see cref="System.Linq.Expressions.ConstantExpression"/> holding the expected type.
        /// </summary>
        private static T Unwrap<T>(this System.Linq.Expressions.Expression? expression)
            where T : RLinq.Expression
            => (T)Unwrap(expression);

        /// <summary>
        /// Unwraps the resulting <see cref="RLinq.Expression"/>. The expression may be null.
        /// </summary>
        private static RLinq.Expression? UnwrapNullable(this System.Linq.Expressions.Expression? expression)
            => expression is ResultWrapperExpression resultWrapperExpression
            ? resultWrapperExpression.Result
            : null;

        private static bool KeepMarkerFunctions(System.Linq.Expressions.Expression expression)
        {
            if (expression is System.Linq.Expressions.MethodCallExpression methodCallExpression &&
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
                    typeof(ConstantQueryArgument),
                    typeof(VariableQueryArgument),
                    typeof(VariableQueryArgumentList),
                    typeof(QueryableResourceDescriptor),
                    typeof(VariableQueryArgument<>),
                    typeof(System.Linq.Expressions.Expression),
                    typeof(IQueryable),
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

        private sealed class LinqExpressionToRemoteExpressionTranslator : ExpressionVisitorBase
        {
            private readonly Dictionary<System.Linq.Expressions.ParameterExpression, RLinq.ParameterExpression> _parameterExpressionCache =
                new Dictionary<System.Linq.Expressions.ParameterExpression, RLinq.ParameterExpression>(ReferenceEqualityComparer<System.Linq.Expressions.ParameterExpression>.Default);

            private readonly Dictionary<System.Linq.Expressions.LabelTarget, RLinq.LabelTarget> _labelTargetCache =
                new Dictionary<System.Linq.Expressions.LabelTarget, RLinq.LabelTarget>(ReferenceEqualityComparer<System.Linq.Expressions.LabelTarget>.Default);

            private readonly Dictionary<object, ConstantQueryArgument> _constantQueryArgumentCache =
                new Dictionary<object, ConstantQueryArgument>(ReferenceEqualityComparer<object>.Default);

            private readonly Func<System.Linq.Expressions.Expression, bool>? _canBeEvaluatedLocally;
            private readonly ITypeInfoProvider _typeInfoProvider;

            public LinqExpressionToRemoteExpressionTranslator(ITypeInfoProvider? typeInfoProvider, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            {
                _canBeEvaluatedLocally = canBeEvaluatedLocally.And(KeepMarkerFunctions);
                _typeInfoProvider = typeInfoProvider ?? new TypeInfoProvider(false, false);
            }

            public RLinq.Expression ToRemoteExpression(System.Linq.Expressions.Expression expression)
            {
                if (expression is null)
                {
                    throw new ArgumentNullException(nameof(expression));
                }

                var partialEvalExpression = expression.PartialEval(_canBeEvaluatedLocally);
                var constExpression = Visit(partialEvalExpression);
                return constExpression.Unwrap();
            }

            [return: NotNullIfNotNull("expression")]
            protected override System.Linq.Expressions.Expression? Visit(System.Linq.Expressions.Expression? expression)
                => expression?.NodeType switch
                {
                    System.Linq.Expressions.ExpressionType.New => VisitNew((System.Linq.Expressions.NewExpression)expression).Wrap(),
                    _ => base.Visit(expression),
                };

            protected override System.Linq.Expressions.Expression VisitSwitch(System.Linq.Expressions.SwitchExpression switchExpression)
            {
                var defaultExpression = Visit(switchExpression.DefaultBody).UnwrapNullable();
                var switchValue = Visit(switchExpression.SwitchValue).Unwrap();
                var cases = (switchExpression.Cases ?? Enumerable.Empty<System.Linq.Expressions.SwitchCase>()).Select(VisitSwitchCase).ToList();
                return new RLinq.SwitchExpression(switchValue, switchExpression.Comparison, defaultExpression, cases).Wrap();
            }

            private new RLinq.SwitchCase VisitSwitchCase(System.Linq.Expressions.SwitchCase switchCase)
            {
                var body = Visit(switchCase.Body).Unwrap();
                var testValues = switchCase.TestValues.Select(Visit).Select(Unwrap).ToList();
                return new RLinq.SwitchCase(body, testValues);
            }

            protected override System.Linq.Expressions.Expression VisitTry(System.Linq.Expressions.TryExpression tryExpression)
            {
                var body = Visit(tryExpression.Body).Unwrap();
                var fault = Visit(tryExpression.Fault).UnwrapNullable();
                var @finally = Visit(tryExpression.Finally).UnwrapNullable();
                var handlers = tryExpression.Handlers?.Select(VisitCatch);
                return new RLinq.TryExpression(_typeInfoProvider.Get(tryExpression.Type), body, fault, @finally, handlers).Wrap();
            }

            private new RLinq.CatchBlock VisitCatch(System.Linq.Expressions.CatchBlock catchBlock)
            {
                var body = Visit(catchBlock.Body).Unwrap();
                var filter = Visit(catchBlock.Filter).UnwrapNullable();
                var variable = Visit(catchBlock.Variable).UnwrapNullable() as RLinq.ParameterExpression;
                return new RLinq.CatchBlock(_typeInfoProvider.Get(catchBlock.Test), variable, body, filter);
            }

            protected override System.Linq.Expressions.Expression VisitListInit(System.Linq.Expressions.ListInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var initializers = VisitElementInitializerList(node.Initializers);
                return new RLinq.ListInitExpression(n, initializers).Wrap();
            }

            private new IEnumerable<RLinq.ElementInit> VisitElementInitializerList(ReadOnlyCollection<System.Linq.Expressions.ElementInit> original)
                => original
                .Select(VisitElementInitializer)
                .ToArray();

            private new RLinq.ElementInit VisitElementInitializer(System.Linq.Expressions.ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments).Select(Unwrap);
                return new RLinq.ElementInit(_typeInfoProvider.GetMethodInfo(initializer.AddMethod), arguments);
            }

            private new RLinq.NewExpression VisitNew(System.Linq.Expressions.NewExpression node)
            {
                IEnumerable<RLinq.Expression>? arguments = null;
                if (node.Arguments?.Count > 0)
                {
                    arguments = node.Arguments
                        .Select(Visit)
                        .Select(Unwrap);
                }

                return node.Constructor is null
                    ? new RLinq.NewExpression(_typeInfoProvider.Get(node.Type))
                    : new RLinq.NewExpression(_typeInfoProvider.GetConstructorInfo(node.Constructor), arguments, node.Members?.Select(x => _typeInfoProvider.GetMemberInfo(x)));
            }

            protected override System.Linq.Expressions.Expression VisitConstant(System.Linq.Expressions.ConstantExpression node)
            {
                RLinq.ConstantExpression exp;
                if (node.Type == typeof(Type) && node.Value is Type typeValue)
                {
                    exp = new RLinq.ConstantExpression(typeValue.AsTypeInfo(), node.Type);
                }
                else if (node.Value != null && ConstantValueMapper.TypeNeedsWrapping(node.Value.GetType()))
                {
                    var key = new { node.Value, node.Type };
                    if (!_constantQueryArgumentCache.TryGetValue(key, out var constantQueryArgument))
                    {
                        var dynamicObject = ConstantValueMapper.ForSubstitution(_typeInfoProvider).MapObject(node.Value);
                        constantQueryArgument = new ConstantQueryArgument(dynamicObject.Type);

                        _constantQueryArgumentCache.Add(key, constantQueryArgument);

                        foreach (var property in dynamicObject.Properties ?? Enumerable.Empty<Property>())
                        {
                            var propertyValue = property.Value;
                            if (propertyValue is System.Linq.Expressions.Expression expressionValue)
                            {
                                propertyValue = Visit(expressionValue).UnwrapNullable();
                            }

                            constantQueryArgument.Add(property.Name, propertyValue);
                        }
                    }

                    exp = node.Type == constantQueryArgument.Type?.Type
                        ? new RLinq.ConstantExpression(constantQueryArgument, constantQueryArgument.Type)
                        : new RLinq.ConstantExpression(constantQueryArgument, _typeInfoProvider.Get(node.Type));
                }
                else
                {
                    exp = new RLinq.ConstantExpression(node.Value, _typeInfoProvider.Get(node.Type));
                }

                return exp.Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitParameter(System.Linq.Expressions.ParameterExpression node)
            {
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(node, out var exp))
                    {
                        exp = new RLinq.ParameterExpression(_typeInfoProvider.Get(node.Type), node.Name, _parameterExpressionCache.Count + 1);
                        _parameterExpressionCache.Add(node, exp);
                    }

                    return exp.Wrap();
                }
            }

            protected override System.Linq.Expressions.Expression VisitBinary(System.Linq.Expressions.BinaryExpression node)
            {
                var binaryOperator = node.NodeType.ToBinaryOperator();
                var left = Visit(node.Left).Unwrap();
                var right = Visit(node.Right).Unwrap();
                var conversion = Visit(node.Conversion).UnwrapNullable() as RLinq.LambdaExpression;
                return new RLinq.BinaryExpression(binaryOperator, left, right, node.IsLiftedToNull, _typeInfoProvider.GetMethodInfo(node.Method), conversion).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitTypeIs(System.Linq.Expressions.TypeBinaryExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                return new RLinq.TypeBinaryExpression(expression, _typeInfoProvider.Get(node.TypeOperand)).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitMemberAccess(System.Linq.Expressions.MemberExpression node)
            {
                var instance = Visit(node.Expression).UnwrapNullable();
                return new RLinq.MemberExpression(instance, _typeInfoProvider.GetMemberInfo(node.Member)).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitMemberInit(System.Linq.Expressions.MemberInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var bindings = VisitBindingList(node.Bindings);
                return new RLinq.MemberInitExpression(n, bindings).Wrap();
            }

            private new IEnumerable<RLinq.MemberBinding> VisitBindingList(ReadOnlyCollection<System.Linq.Expressions.MemberBinding> original)
                => original
                .Select(x => VisitMemberBinding(x))
                .ToArray();

            private new RLinq.MemberBinding VisitMemberBinding(System.Linq.Expressions.MemberBinding binding)
                => binding.BindingType switch
                {
                    System.Linq.Expressions.MemberBindingType.Assignment => VisitMemberAssignment((System.Linq.Expressions.MemberAssignment)binding),
                    System.Linq.Expressions.MemberBindingType.MemberBinding => VisitMemberMemberBinding((System.Linq.Expressions.MemberMemberBinding)binding),
                    System.Linq.Expressions.MemberBindingType.ListBinding => VisitMemberListBinding((System.Linq.Expressions.MemberListBinding)binding),
                    _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
                };

            private new RLinq.MemberAssignment VisitMemberAssignment(System.Linq.Expressions.MemberAssignment assignment)
            {
                var expression = Visit(assignment.Expression).Unwrap();
                var member = _typeInfoProvider.GetMemberInfo(assignment.Member);
                return new RLinq.MemberAssignment(member, expression);
            }

            private new RLinq.MemberMemberBinding VisitMemberMemberBinding(System.Linq.Expressions.MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RLinq.MemberMemberBinding(m, bindings);
            }

            private new RLinq.MemberListBinding VisitMemberListBinding(System.Linq.Expressions.MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = _typeInfoProvider.GetMemberInfo(binding.Member);
                return new RLinq.MemberListBinding(m, initializers);
            }

            protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression node)
            {
                var instance = Visit(node.Object).UnwrapNullable();
                var arguments = node.Arguments
                    .Select(Visit)
                    .Select(Unwrap);
                return new RLinq.MethodCallExpression(instance, _typeInfoProvider.GetMethodInfo(node.Method), arguments).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitLambda(System.Linq.Expressions.LambdaExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var parameters = node.Parameters
                    .Select(VisitParameter)
                    .Select(Unwrap<RLinq.ParameterExpression>);
                return new RLinq.LambdaExpression(_typeInfoProvider.Get(node.Type), body, parameters).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitUnary(System.Linq.Expressions.UnaryExpression node)
            {
                var unaryOperator = node.NodeType.ToUnaryOperator();
                var operand = Visit(node.Operand).Unwrap();
                return new RLinq.UnaryExpression(unaryOperator, operand, _typeInfoProvider.Get(node.Type), _typeInfoProvider.GetMethodInfo(node.Method)).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitConditional(System.Linq.Expressions.ConditionalExpression node)
            {
                var test = Visit(node.Test).Unwrap();
                var ifTrue = Visit(node.IfTrue).Unwrap();
                var ifFalse = Visit(node.IfFalse).Unwrap();
                return new RLinq.ConditionalExpression(test, ifTrue, ifFalse).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitNewArray(System.Linq.Expressions.NewArrayExpression node)
            {
                var newArrayType = node.NodeType.ToNewArrayType();
                var expressions = VisitExpressionList(node.Expressions).Select(Unwrap);
                var elementType = TypeHelper.GetElementType(node.Type) ?? throw new RemoteLinqException($"Failed to get element type of {node.Type}.");
                return new RLinq.NewArrayExpression(newArrayType, _typeInfoProvider.Get(elementType), expressions).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitInvocation(System.Linq.Expressions.InvocationExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                var arguments = VisitExpressionList(node.Arguments)?.Select(Unwrap);
                return new RLinq.InvokeExpression(expression, arguments).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitBlock(System.Linq.Expressions.BlockExpression node)
            {
                var expressions = VisitExpressionList(node.Expressions)?.Select(Unwrap);
                IEnumerable<RLinq.ParameterExpression>? variables = null;
                if (node.Variables != null)
                {
                    var nodeVariables = node.Variables.Cast<System.Linq.Expressions.Expression>().ToList().AsReadOnly();
                    variables = VisitExpressionList(nodeVariables)?.Select(Unwrap<RLinq.ParameterExpression>);
                }

                var type = node.Type == node.Result.Type ? null : node.Type;
                return new RLinq.BlockExpression(_typeInfoProvider.Get(type), variables, expressions).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitDefault(System.Linq.Expressions.DefaultExpression node)
            {
                return new RLinq.DefaultExpression(_typeInfoProvider.Get(node.Type)).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitLabel(System.Linq.Expressions.LabelExpression node)
            {
                var target = VisitTarget(node.Target);
                var defaultValue = Visit(node.DefaultValue).UnwrapNullable();
                return new RLinq.LabelExpression(target, defaultValue).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitLoop(System.Linq.Expressions.LoopExpression node)
            {
                var body = Visit(node.Body).UnwrapNullable();
                var breakLabel = VisitTarget(node.BreakLabel);
                var continueLabel = VisitTarget(node.ContinueLabel);
                return new RLinq.LoopExpression(body, breakLabel, continueLabel).Wrap();
            }

            protected override System.Linq.Expressions.Expression VisitGoto(System.Linq.Expressions.GotoExpression node)
            {
                var kind = node.Kind.ToGotoExpressionKind();
                var target = VisitTarget(node.Target);
                var type = node.Target.Type == node.Type ? null : node.Type;
                var value = Visit(node.Value).UnwrapNullable();
                return new RLinq.GotoExpression(kind, target, _typeInfoProvider.Get(type), value).Wrap();
            }

            [return: NotNullIfNotNull("labelTarget")]
            private RLinq.LabelTarget? VisitTarget(System.Linq.Expressions.LabelTarget? labelTarget)
            {
                if (labelTarget is null)
                {
                    return null;
                }

                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out var target))
                    {
                        target = new RLinq.LabelTarget(labelTarget.Name, _typeInfoProvider.Get(labelTarget.Type), _labelTargetCache.Count + 1);
                        _labelTargetCache.Add(labelTarget, target);
                    }

                    return target;
                }
            }
        }

        private sealed class RemoteExpressionToLinqExpressionTranslator : IEqualityComparer<RLinq.ParameterExpression>, IEqualityComparer<RLinq.LabelTarget>
        {
            private readonly Dictionary<RLinq.ParameterExpression, System.Linq.Expressions.ParameterExpression> _parameterExpressionCache;
            private readonly Dictionary<RLinq.LabelTarget, System.Linq.Expressions.LabelTarget> _labelTargetCache;
            private readonly ITypeResolver _typeResolver;

            public RemoteExpressionToLinqExpressionTranslator(ITypeResolver? typeResolver)
            {
                _parameterExpressionCache = new Dictionary<RLinq.ParameterExpression, System.Linq.Expressions.ParameterExpression>(this);
                _labelTargetCache = new Dictionary<RLinq.LabelTarget, System.Linq.Expressions.LabelTarget>(this);
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            public System.Linq.Expressions.Expression ToExpression(RLinq.Expression expression) => Visit(expression ?? throw new ArgumentNullException(nameof(expression)));

            [return: NotNullIfNotNull("expression")]
            private System.Linq.Expressions.Expression? Visit(RLinq.Expression? expression)
                => expression?.NodeType switch
                {
                    null => null,
                    RLinq.ExpressionType.Binary => VisitBinary((RLinq.BinaryExpression)expression),
                    RLinq.ExpressionType.Block => VisitBlock((RLinq.BlockExpression)expression),
                    RLinq.ExpressionType.Call => VisitMethodCall((RLinq.MethodCallExpression)expression),
                    RLinq.ExpressionType.Conditional => VisitConditional((RLinq.ConditionalExpression)expression),
                    RLinq.ExpressionType.Constant => VisitConstant((RLinq.ConstantExpression)expression),
                    RLinq.ExpressionType.Default => VisitDefault((RLinq.DefaultExpression)expression),
                    RLinq.ExpressionType.Invoke => VisitInvoke((RLinq.InvokeExpression)expression),
                    RLinq.ExpressionType.Goto => VisitGoto((RLinq.GotoExpression)expression),
                    RLinq.ExpressionType.Label => VisitLabel((RLinq.LabelExpression)expression),
                    RLinq.ExpressionType.Lambda => VisitLambda((RLinq.LambdaExpression)expression),
                    RLinq.ExpressionType.ListInit => VisitListInit((RLinq.ListInitExpression)expression),
                    RLinq.ExpressionType.Loop => VisitLoop((RLinq.LoopExpression)expression),
                    RLinq.ExpressionType.MemberAccess => VisitMember((RLinq.MemberExpression)expression),
                    RLinq.ExpressionType.MemberInit => VisitMemberInit((RLinq.MemberInitExpression)expression),
                    RLinq.ExpressionType.New => VisitNew((RLinq.NewExpression)expression),
                    RLinq.ExpressionType.NewArray => VisitNewArray((RLinq.NewArrayExpression)expression),
                    RLinq.ExpressionType.Parameter => VisitParameter((RLinq.ParameterExpression)expression),
                    RLinq.ExpressionType.Switch => VisitSwitch((RLinq.SwitchExpression)expression),
                    RLinq.ExpressionType.Try => VisitTry((RLinq.TryExpression)expression),
                    RLinq.ExpressionType.TypeIs => VisitTypeIs((RLinq.TypeBinaryExpression)expression),
                    RLinq.ExpressionType.Unary => VisitUnary((RLinq.UnaryExpression)expression),
                    _ => throw new NotSupportedException($"Unknown expression note type: '{expression.NodeType}'"),
                };

            private System.Linq.Expressions.Expression VisitSwitch(RLinq.SwitchExpression switchExpression)
            {
                var defaultExpression = Visit(switchExpression.DefaultExpression);
                var switchValue = Visit(switchExpression.SwitchValue);
                var compareMethod = switchExpression.Comparison.ResolveMethod(_typeResolver);
                var cases = switchExpression.Cases.Select(VisitSwitchCase);

                return System.Linq.Expressions.Expression.Switch(switchValue, defaultExpression, compareMethod, cases);
            }

            private System.Linq.Expressions.SwitchCase VisitSwitchCase(RLinq.SwitchCase switchCase)
            {
                var body = Visit(switchCase.Body);
                var testCases = switchCase.TestValues ?? Enumerable.Empty<RLinq.Expression>();
                return System.Linq.Expressions.Expression.SwitchCase(body, testCases.Select(Visit));
            }

            private System.Linq.Expressions.Expression VisitTry(RLinq.TryExpression tryExpression)
            {
                var body = Visit(tryExpression.Body);
                var type = tryExpression.Type is null ? null : _typeResolver.ResolveType(tryExpression.Type);
                var fault = tryExpression.Fault is null ? null : Visit(tryExpression.Fault);
                var @finally = tryExpression.Finally is null ? null : Visit(tryExpression.Finally);
                var handlers = tryExpression.Handlers?.Select(VisitCatchBlock) ?? Enumerable.Empty<System.Linq.Expressions.CatchBlock>();

                return System.Linq.Expressions.Expression.MakeTry(type, body, @finally, fault, handlers);
            }

            private System.Linq.Expressions.CatchBlock VisitCatchBlock(RLinq.CatchBlock catchBlock)
            {
                var exceptionType = catchBlock.Test is null ? null : _typeResolver.ResolveType(catchBlock.Test);
                var exceptionParameter = catchBlock.Variable is null ? null : VisitParameter(catchBlock.Variable);
                var body = catchBlock.Body is null ? null : Visit(catchBlock.Body);
                var filter = catchBlock.Filter is null ? null : Visit(catchBlock.Filter);

                return System.Linq.Expressions.Expression.MakeCatchBlock(exceptionType, exceptionParameter, body, filter);
            }

            private System.Linq.Expressions.NewExpression VisitNew(RLinq.NewExpression newExpression)
            {
                if (newExpression.Constructor is null)
                {
                    var type = newExpression.Type.ResolveType(_typeResolver);
                    return System.Linq.Expressions.Expression.New(type);
                }

                var constructor = newExpression.Constructor.ResolveConstructor(_typeResolver);
                if (newExpression.Arguments is null)
                {
                    if (newExpression.Members?.Any() ?? false)
                    {
                        var members = newExpression.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                        return System.Linq.Expressions.Expression.New(constructor, new System.Linq.Expressions.Expression[0], members);
                    }
                    else
                    {
                        return System.Linq.Expressions.Expression.New(constructor);
                    }
                }
                else
                {
                    var arguments =
                        from a in newExpression.Arguments
                        select Visit(a);
                    if (newExpression.Members?.Any() ?? false)
                    {
                        var members = newExpression.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                        return System.Linq.Expressions.Expression.New(constructor, arguments, members);
                    }
                    else
                    {
                        return System.Linq.Expressions.Expression.New(constructor, arguments);
                    }
                }
            }

            private System.Linq.Expressions.Expression VisitNewArray(RLinq.NewArrayExpression expression)
            {
                var expressions = VisitExpressionList(expression.Expressions);
                var type = _typeResolver.ResolveType(expression.Type);
                return expression.NewArrayType switch
                {
                    RLinq.NewArrayType.NewArrayBounds => System.Linq.Expressions.Expression.NewArrayBounds(type, expressions),
                    RLinq.NewArrayType.NewArrayInit => System.Linq.Expressions.Expression.NewArrayInit(type, expressions),
                    _ => throw new NotSupportedException($"Unhandled new array type {expression.NewArrayType}"),
                };
            }

            private System.Linq.Expressions.Expression VisitMemberInit(RLinq.MemberInitExpression expression)
            {
                var n = VisitNew(expression.NewExpression);
                var bindings = VisitBindingList(expression.Bindings);
                return System.Linq.Expressions.Expression.MemberInit(n, bindings);
            }

            private System.Linq.Expressions.Expression VisitInvoke(RLinq.InvokeExpression node)
            {
                var expression = Visit(node.Expression);
                var arguments =
                    from i in node.Arguments ?? Enumerable.Empty<RLinq.Expression>()
                    select Visit(i);
                return System.Linq.Expressions.Expression.Invoke(expression, arguments);
            }

            private System.Linq.Expressions.Expression VisitBlock(RLinq.BlockExpression node)
            {
                var type = node.Type.ResolveType(_typeResolver);
                var variables = node.Variables?.Select(VisitParameter).AsEmptyIfNull();
                var expressions = node.Expressions?.Select(Visit).AsEmptyIfNull();
                return type is null
                    ? System.Linq.Expressions.Expression.Block(variables, expressions)
                    : System.Linq.Expressions.Expression.Block(type, variables, expressions);
            }

            private IEnumerable<System.Linq.Expressions.MemberBinding> VisitBindingList(IEnumerable<RLinq.MemberBinding> original)
            {
                var list =
                    from i in original
                    select VisitMemberBinding(i);
                return list.ToArray();
            }

            private System.Linq.Expressions.MemberBinding VisitMemberBinding(RLinq.MemberBinding binding)
                => binding.BindingType switch
                {
                    RLinq.MemberBindingType.Assignment => VisitMemberAssignment((RLinq.MemberAssignment)binding),
                    RLinq.MemberBindingType.MemberBinding => VisitMemberMemberBinding((RLinq.MemberMemberBinding)binding),
                    RLinq.MemberBindingType.ListBinding => VisitMemberListBinding((RLinq.MemberListBinding)binding),
                    _ => throw new NotSupportedException($"Unhandled binding type '{binding.BindingType}'"),
                };

            private System.Linq.Expressions.MemberAssignment VisitMemberAssignment(RLinq.MemberAssignment assignment)
            {
                var e = Visit(assignment.Expression);
                var m = assignment.Member.ResolveMemberInfo(_typeResolver);
                return System.Linq.Expressions.Expression.Bind(m, e);
            }

            private System.Linq.Expressions.MemberMemberBinding VisitMemberMemberBinding(RLinq.MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = binding.Member.ResolveMemberInfo(_typeResolver);
                return System.Linq.Expressions.Expression.MemberBind(m, bindings);
            }

            private System.Linq.Expressions.MemberListBinding VisitMemberListBinding(RLinq.MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = binding.Member.ResolveMemberInfo(_typeResolver);
                return System.Linq.Expressions.Expression.ListBind(m, initializers);
            }

            private IEnumerable<System.Linq.Expressions.ElementInit> VisitElementInitializerList(IEnumerable<RLinq.ElementInit> original)
                => original
                .Select(VisitElementInitializer)
                .ToArray();

            private System.Linq.Expressions.ElementInit VisitElementInitializer(RLinq.ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments);
                var m = initializer.AddMethod.ResolveMethod(_typeResolver);
                return System.Linq.Expressions.Expression.ElementInit(m, arguments);
            }

            private IEnumerable<System.Linq.Expressions.Expression> VisitExpressionList(IEnumerable<RLinq.Expression> original)
                => original
                .Select(x => Visit(x))
                .ToArray();

            private System.Linq.Expressions.Expression VisitListInit(RLinq.ListInitExpression listInitExpression)
            {
                var n = VisitNew(listInitExpression.NewExpression);
                var initializers =
                    from i in listInitExpression.Initializers
                    select System.Linq.Expressions.Expression.ElementInit(i.AddMethod.ResolveMethod(_typeResolver), i.Arguments.Select(Visit));
                return System.Linq.Expressions.Expression.ListInit(n, initializers);
            }

            private System.Linq.Expressions.ParameterExpression VisitParameter(RLinq.ParameterExpression parameterExpression)
            {
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(parameterExpression, out var exp))
                    {
                        var type = _typeResolver.ResolveType(parameterExpression.ParameterType);
                        exp = System.Linq.Expressions.Expression.Parameter(type, parameterExpression.ParameterName);
                        _parameterExpressionCache.Add(parameterExpression, exp);
                    }

                    return exp;
                }
            }

            private System.Linq.Expressions.Expression VisitUnary(RLinq.UnaryExpression unaryExpression)
            {
                var expressionType = unaryExpression.UnaryOperator.ToExpressionType();
                var exp = Visit(unaryExpression.Operand);
                var type = unaryExpression.Type is null ? null : _typeResolver.ResolveType(unaryExpression.Type);
                var method = unaryExpression.Method?.ResolveMethod(_typeResolver);
                return System.Linq.Expressions.Expression.MakeUnary(expressionType, exp, type, method);
            }

            private System.Linq.Expressions.Expression VisitMember(RLinq.MemberExpression memberExpression)
            {
                var exp = Visit(memberExpression.Expression);
                var m = memberExpression.Member.ResolveMemberInfo(_typeResolver);
                return System.Linq.Expressions.Expression.MakeMemberAccess(exp, m);
            }

            private System.Linq.Expressions.Expression VisitMethodCall(RLinq.MethodCallExpression methodCallExpression)
            {
                var instance = Visit(methodCallExpression.Instance);
                var arguments = methodCallExpression.Arguments?
                    .Select(x => Visit(x))
                    .ToArray();
                var methodInfo = methodCallExpression.Method.ResolveMethod(_typeResolver);
                return System.Linq.Expressions.Expression.Call(instance, methodInfo, arguments);
            }

            private System.Linq.Expressions.Expression VisitConditional(RLinq.ConditionalExpression expression)
            {
                var test = Visit(expression.Test);
                var ifTrue = Visit(expression.IfTrue);
                var ifFalse = Visit(expression.IfFalse);

                if (ifFalse is System.Linq.Expressions.DefaultExpression && ifFalse.Type == typeof(void))
                {
                    return System.Linq.Expressions.Expression.IfThen(test, ifTrue);
                }

                return System.Linq.Expressions.Expression.Condition(test, ifTrue, ifFalse);
            }

            private System.Linq.Expressions.Expression VisitConstant(RLinq.ConstantExpression constantValueExpression)
            {
                var value = constantValueExpression.Value;
                var type = constantValueExpression.Type.ResolveType(_typeResolver);

                if (type == typeof(Type) && value is Aqua.TypeSystem.TypeInfo typeInfo)
                {
                    value = typeInfo.ResolveType(_typeResolver);
                }
                else if (value is ConstantQueryArgument oldConstantQueryArgument && oldConstantQueryArgument.Type != null)
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(oldConstantQueryArgument.Type);
                    foreach (var property in oldConstantQueryArgument.Properties ?? Enumerable.Empty<Property>())
                    {
                        var propertyValue = property.Value;
                        if (propertyValue is RLinq.Expression expressionValue)
                        {
                            propertyValue = Visit(expressionValue);
                        }

                        newConstantQueryArgument.Add(property.Name, propertyValue);
                    }

                    value = ConstantValueMapper.ForReconstruction(_typeResolver).Map(newConstantQueryArgument, type);
                }
                else if (value is string && type != null && type != typeof(string))
                {
                    var mapper = new DynamicObjectMapper();
                    var obj = mapper.MapObject(value);
                    value = mapper.Map(obj, type);
                }

                return System.Linq.Expressions.Expression.Constant(value, type);
            }

            private System.Linq.Expressions.Expression VisitBinary(RLinq.BinaryExpression binaryExpression)
            {
                var p1 = Visit(binaryExpression.LeftOperand);
                var p2 = Visit(binaryExpression.RightOperand);
                var conversion = Visit(binaryExpression.Conversion) as System.Linq.Expressions.LambdaExpression;
                var binaryType = binaryExpression.BinaryOperator.ToExpressionType();
                var method = binaryExpression.Method is null ? null : binaryExpression.Method.ResolveMethod(_typeResolver);
                return System.Linq.Expressions.Expression.MakeBinary(binaryType, p1, p2, binaryExpression.IsLiftedToNull, method, conversion);
            }

            private System.Linq.Expressions.Expression VisitTypeIs(RLinq.TypeBinaryExpression typeBinaryExpression)
            {
                var expression = Visit(typeBinaryExpression.Expression);
                var type = _typeResolver.ResolveType(typeBinaryExpression.TypeOperand);
                return System.Linq.Expressions.Expression.TypeIs(expression, type);
            }

            private System.Linq.Expressions.Expression VisitLambda(RLinq.LambdaExpression lambdaExpression)
            {
                var body = Visit(lambdaExpression.Expression);
                var parameters = lambdaExpression.Parameters?.Select(VisitParameter) ?? Enumerable.Empty<System.Linq.Expressions.ParameterExpression>();

                if (lambdaExpression.Type is null)
                {
                    return System.Linq.Expressions.Expression.Lambda(body, parameters);
                }

                var delegateType = _typeResolver.ResolveType(lambdaExpression.Type);
                return System.Linq.Expressions.Expression.Lambda(delegateType, body, parameters);
            }

            private System.Linq.Expressions.Expression VisitDefault(RLinq.DefaultExpression defaultExpression)
            {
                var type = defaultExpression.Type.ResolveType(_typeResolver);
                return System.Linq.Expressions.Expression.Default(type);
            }

            private System.Linq.Expressions.Expression VisitGoto(RLinq.GotoExpression gotoExpression)
            {
                var kind = gotoExpression.Kind.ToGotoExpressionKind();
                var target = VisitTarget(gotoExpression.Target);
                var value = Visit(gotoExpression.Value);
                var type = gotoExpression.Type.ResolveType(_typeResolver);
                return System.Linq.Expressions.Expression.MakeGoto(kind, target, value, type ?? target.Type);
            }

            private System.Linq.Expressions.Expression VisitLabel(RLinq.LabelExpression labelExpression)
            {
                var target = VisitTarget(labelExpression.Target);
                var defaultValue = Visit(labelExpression.DefaultValue);
                return System.Linq.Expressions.Expression.Label(target, defaultValue);
            }

            private System.Linq.Expressions.Expression VisitLoop(RLinq.LoopExpression loopExpression)
            {
                var body = Visit(loopExpression.Body);
                var breakLabel = VisitTarget(loopExpression.BreakLabel);
                var continueLabel = VisitTarget(loopExpression.ContinueLabel);
                return System.Linq.Expressions.Expression.Loop(body, breakLabel, continueLabel);
            }

            [return: NotNullIfNotNull("labelTarget")]
            private System.Linq.Expressions.LabelTarget? VisitTarget(RLinq.LabelTarget? labelTarget)
            {
                if (labelTarget is null)
                {
                    return null;
                }

                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out var target))
                    {
                        var targetType = labelTarget.Type.ResolveType(_typeResolver);
                        target = System.Linq.Expressions.Expression.Label(targetType, labelTarget.Name);
                        _labelTargetCache.Add(labelTarget, target);
                    }

                    return target;
                }
            }

            bool IEqualityComparer<RLinq.ParameterExpression>.Equals(RLinq.ParameterExpression x, RLinq.ParameterExpression y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    if (y is null)
                    {
                        return true;
                    }

                    return false;
                }

                return x.InstanceId == y.InstanceId;
            }

            int IEqualityComparer<RLinq.ParameterExpression>.GetHashCode(RLinq.ParameterExpression obj) => obj?.InstanceId ?? 0;

            bool IEqualityComparer<RLinq.LabelTarget>.Equals(RLinq.LabelTarget x, RLinq.LabelTarget y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    if (y is null)
                    {
                        return true;
                    }

                    return false;
                }

                return x.InstanceId == y.InstanceId;
            }

            int IEqualityComparer<RLinq.LabelTarget>.GetHashCode(RLinq.LabelTarget obj) => obj?.InstanceId ?? 0;
        }
    }
}
