// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua;
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using RLinq = Remote.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionTranslator
    {
        /// <summary>
        /// Translates a given expression into a remote linq expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RLinq.Expression ToRemoteLinqExpression(this Expression expression)
        {
            return new LinqExpressionToRemoteExpressionTranslator().ToRemoteExpression(expression);
        }

        /// <summary>
        /// Translates a given lambda expression into a remote linq expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static RLinq.LambdaExpression ToRemoteLinqExpression(this LambdaExpression expression)
        {
            var lambdaExpression = new LinqExpressionToRemoteExpressionTranslator().ToRemoteExpression(expression);
            return (RLinq.LambdaExpression)lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into an expression
        /// </summary>
        public static Expression ToLinqExpression(this RLinq.Expression expression)
        {
            return ToLinqExpression(expression, null);
        }

        /// <summary>
        /// Translates a given query expression into an expression
        /// </summary>
        public static Expression ToLinqExpression(this RLinq.Expression expression, ITypeResolver typeResolver)
        {
            var exp = new RemoteExpressionToLinqExpressionTranslator(typeResolver).ToExpression(expression);
            return exp;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<Func<T, TResult>> ToLinqExpression<T, TResult>(this RLinq.LambdaExpression expression)
        {
            var exp = expression.ToLinqExpression();
            var lambdaExpression = Expression.Lambda<Func<T, TResult>>(exp.Body, exp.Parameters);
            return lambdaExpression;
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression
        /// </summary>
        public static LambdaExpression ToLinqExpression(this RLinq.LambdaExpression expression)
        {
            return ToLinqExpression(expression, null);
        }

        /// <summary>
        /// Translates a given query expression into a lambda expression
        /// </summary>
        public static LambdaExpression ToLinqExpression(this RLinq.LambdaExpression expression, ITypeResolver typeResolver)
        {
            var lambdaExpression = new RemoteExpressionToLinqExpressionTranslator(typeResolver).ToExpression(expression);
            return (LambdaExpression)lambdaExpression;
        }

        private static ExpressionType ToExpressionType(this RLinq.BinaryOperator binaryOperator) => (ExpressionType)(int)binaryOperator;

        private static ExpressionType ToExpressionType(this RLinq.UnaryOperator unaryOperator) => (ExpressionType)(int)unaryOperator;

        private static GotoExpressionKind ToGotoExpressionKind(this RLinq.GotoExpressionKind kind) => (GotoExpressionKind)(int)kind;

        private static RLinq.BinaryOperator ToBinaryOperator(this ExpressionType expressionType) => (RLinq.BinaryOperator)(int)expressionType;

        private static RLinq.UnaryOperator ToUnaryOperator(this ExpressionType expressionType) => (RLinq.UnaryOperator)(int)expressionType;

        private static RLinq.NewArrayType ToNewArrayType(this ExpressionType expressionType) => (RLinq.NewArrayType)(int)expressionType;

        private static RLinq.GotoExpressionKind ToGotoExpressionKind(this GotoExpressionKind kind) => (RLinq.GotoExpressionKind)(int)kind;

        private static ConstantExpression Wrap(this RLinq.Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : Expression.Constant(expression);
        }

        private static RLinq.ConstantExpression Wrap(this Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : new RLinq.ConstantExpression(expression);
        }

        private static RLinq.Expression Unwrap(this Expression expression)
        {
            if (!ReferenceEquals(null, expression) && expression.NodeType == ExpressionType.Constant && typeof(RLinq.Expression).IsAssignableFrom(expression.Type))
            {
                return (RLinq.Expression)((ConstantExpression)expression).Value;
            }
            else
            {
                return null;
            }
        }

        private static Expression Unwrap(this RLinq.Expression expression)
        {
            if (!ReferenceEquals(null, expression) && expression.NodeType == RLinq.ExpressionType.Constant && ((RLinq.ConstantExpression)expression).Value is System.Linq.Expressions.Expression)
            {
                return (Expression)((RLinq.ConstantExpression)expression).Value;
            }
            else
            {
                return null;
            }
        }

        private static bool KeepMarkerFunctions(Expression expression)
        {
            if (!ExpressionEvaluator.CanBeEvaluatedLocally(expression))
            {
                return false;
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (!ReferenceEquals(null, methodCallExpression))
            {
                if (methodCallExpression.Method.IsGenericMethod &&
                    methodCallExpression.Method.GetGenericMethodDefinition() == MethodInfos.QueryFuntion.Include)
                {
                    return false;
                }
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
                .SelectMany(x => x.IsValueType() ? new[] { x, typeof(Nullable<>).MakeGenericType(x) } : new[] { x })
                .ToDictionary(x => x, x => (object)null)
                .ContainsKey;

            private static readonly Type[] _knownTypes = new[]
                {
                    typeof(ConstantQueryArgument),
                    typeof(VariableQueryArgument),
                    typeof(VariableQueryArgumentList),
                    typeof(QueryableResourceDescriptor),
                    typeof(VariableQueryArgument<>),
                    typeof(Expression),
                    typeof(IQueryable),
                };                                      

            private static readonly Func<Type, bool> _isKnownType = t => _knownTypes.Any(x => x.IsAssignableFrom(t));

            private sealed class IsKnownTypeProvider : IIsKnownTypeProvider
            {
                private readonly bool _includePrimitiveType;

                public IsKnownTypeProvider(bool includePrimitiveType)
                {
                    _includePrimitiveType = includePrimitiveType;
                }

                public bool IsKnownType(Type type) => !TypeNeedsWrapping(type, _includePrimitiveType);
            }

            private ConstantValueMapper(ITypeResolver typeResolver, IIsKnownTypeProvider isKnownTypeProvider)
                : base(typeResolver: typeResolver, isKnownTypeProvider: isKnownTypeProvider)
            {
            }

            public static ConstantValueMapper ForSubstitution()
                => new ConstantValueMapper(null, new IsKnownTypeProvider(true));

            public static ConstantValueMapper ForReconstruction(ITypeResolver typeResolver)
                => new ConstantValueMapper(typeResolver, new IsKnownTypeProvider(false));

            protected override DynamicObject MapToDynamicObjectGraph(object obj, Func<Type, bool> setTypeInformation)
            {
                if (IsEnumerableQuery(obj))
                {
                    obj = Enumerate(obj);
                }

                return base.MapToDynamicObjectGraph(obj, setTypeInformation);
            }

            private static bool IsEnumerableQuery(object obj)
            {
                var type = obj?.GetType();
                return (type?.IsGenericType() ?? false) 
                    && type.GetGenericTypeDefinition() == typeof(EnumerableQuery<>);
            }

            private static object Enumerate(object obj)
                => MethodInfos.Enumerable.ToArray
                    .MakeGenericMethod(((IQueryable)obj).ElementType)
                    .Invoke(null, new[] { obj });

            public static bool TypeNeedsWrapping(Type type, bool includePrimitiveType = true)
            {
                if (includePrimitiveType && _isPrimitiveType(type))
                {
                    return false;
                }

                if (type.IsGenericType())
                {
                    type = type.GetGenericTypeDefinition();
                }

                if (type == typeof(EnumerableQuery<>))
                {
                    return true;
                }

                if (_isKnownType(type))
                {
                    return false;
                }

                return true;
            }
        }


        private sealed class LinqExpressionToRemoteExpressionTranslator : ExpressionVisitorBase
        {
            private readonly Dictionary<ParameterExpression, RLinq.ParameterExpression> _parameterExpressionCache =
                new Dictionary<ParameterExpression, RLinq.ParameterExpression>(ReferenceEqualityComparer<ParameterExpression>.Default);

            private readonly Dictionary<LabelTarget, RLinq.LabelTarget> _labelTargetCache =
                new Dictionary<LabelTarget, RLinq.LabelTarget>(ReferenceEqualityComparer<LabelTarget>.Default);

            private readonly Dictionary<object, ConstantQueryArgument> _constantQueryArgumentCache =
                new Dictionary<object, ConstantQueryArgument>(ReferenceEqualityComparer<object>.Default);

            public RLinq.Expression ToRemoteExpression(Expression expression)
            {
                var partialEvalExpression = expression.PartialEval(KeepMarkerFunctions);
                if (partialEvalExpression == null)
                {
                    throw CreateNotSupportedException(expression);
                }

                var constExpression = Visit(partialEvalExpression);
                return constExpression.Unwrap();
            }

            protected override Expression Visit(Expression expression)
            {
                if (ReferenceEquals(null, expression))
                {
                    return expression;
                }

                switch (expression.NodeType)
                {
                    case ExpressionType.New:
                        return VisitNew((NewExpression)expression).Wrap();

                    default:
                        return base.Visit(expression);
                }
            }

            protected override Expression VisitTry(TryExpression tryExpression)
            {
                RLinq.Expression body = Visit(tryExpression.Body).Unwrap();
                RLinq.Expression fault = Visit(tryExpression.Fault).Unwrap();
                RLinq.Expression @finally = Visit(tryExpression.Finally).Unwrap();
                List<RLinq.CatchBlock> handlers = (tryExpression.Handlers?.Select(VisitCatch) ?? Enumerable.Empty<RLinq.CatchBlock>())?.ToList();
                return new RLinq.TryExpression(tryExpression.Type, body, fault, @finally, handlers).Wrap();
            }

            private new RLinq.CatchBlock VisitCatch(CatchBlock catchBlock)
            {
                RLinq.Expression body = Visit(catchBlock.Body).Unwrap();
                RLinq.Expression filter = Visit(catchBlock.Filter).Unwrap();
                RLinq.ParameterExpression variable = Visit(catchBlock.Variable).Unwrap() as RLinq.ParameterExpression;
                return new RLinq.CatchBlock(catchBlock.Test, variable, body, filter);
            }

            protected override Expression VisitListInit(ListInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var initializers = VisitElementInitializerList(node.Initializers);
                return new RLinq.ListInitExpression(n, initializers).Wrap();
            }

            private new IEnumerable<RLinq.ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
            {
                var list =
                    from i in original
                    select VisitElementInitializer(i);
                return list.ToArray();
            }

            private new RLinq.ElementInit VisitElementInitializer(ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments);
                var rlinqArguments =
                    from i in arguments
                    select i.Unwrap();
                return new RLinq.ElementInit(initializer.AddMethod, rlinqArguments);
            }

            private new RLinq.NewExpression VisitNew(NewExpression node)
            {
                var arguments = default(IEnumerable<RLinq.Expression>);
                if (node.Arguments != null && node.Arguments.Count > 0)
                {
                    arguments =
                        from arg in node.Arguments
                        select Visit(arg).Unwrap();
                }

                return new RLinq.NewExpression(node.Constructor, arguments, node.Members);
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                RLinq.ConstantExpression exp;
                if (!ReferenceEquals(null, node.Value) && ConstantValueMapper.TypeNeedsWrapping(node.Type))
                {
                    var key = new { node.Value, node.Type };
                    ConstantQueryArgument constantQueryArgument;
                    if (!_constantQueryArgumentCache.TryGetValue(key, out constantQueryArgument))
                    {
                        var dynamicObject = ConstantValueMapper.ForSubstitution().MapObject(node.Value);
                        constantQueryArgument = new ConstantQueryArgument(dynamicObject.Type);

                        _constantQueryArgumentCache.Add(key, constantQueryArgument);

                        foreach (var property in dynamicObject.Properties)
                        {
                            var propertyValue = property.Value;
                            var expressionValue = propertyValue as Expression;
                            if (!ReferenceEquals(null, expressionValue))
                            {
                                propertyValue = Visit(expressionValue).Unwrap();
                            }

                            constantQueryArgument.Add(property.Name, propertyValue);
                        }
                    }

                    if (node.Value?.GetType() == node.Type)
                    {
                        exp = new RLinq.ConstantExpression(constantQueryArgument, constantQueryArgument.Type);
                    }
                    else
                    {
                        exp = new RLinq.ConstantExpression(constantQueryArgument, node.Type);
                    }
                }
                else
                {
                    exp = new RLinq.ConstantExpression(node.Value, node.Type);
                }

                return exp.Wrap();
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                RLinq.ParameterExpression exp;
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(node, out exp))
                    {
                        exp = new RLinq.ParameterExpression(node.Type, node.Name, _parameterExpressionCache.Count + 1);
                        _parameterExpressionCache.Add(node, exp);
                    }
                }

                return exp.Wrap();
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var binaryOperator = node.NodeType.ToBinaryOperator();
                var left = Visit(node.Left).Unwrap();
                var right = Visit(node.Right).Unwrap();
                var conversion = Visit(node.Conversion).Unwrap() as RLinq.LambdaExpression;
                return new RLinq.BinaryExpression(binaryOperator, left, right, node.IsLiftedToNull, node.Method, conversion).Wrap();
            }

            protected override Expression VisitTypeIs(TypeBinaryExpression node)
            {
                var expression = Visit(node.Expression).Unwrap();
                return new RLinq.TypeBinaryExpression(expression, node.TypeOperand).Wrap();
            }

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                var instance = Visit(node.Expression).Unwrap();
                return new RLinq.MemberExpression(instance, node.Member).Wrap();
            }

            protected override Expression VisitMemberInit(MemberInitExpression node)
            {
                var n = VisitNew(node.NewExpression);
                var bindings = VisitBindingList(node.Bindings);
                return new RLinq.MemberInitExpression(n, bindings).Wrap();
            }

            private new IEnumerable<RLinq.MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
            {
                var list =
                    from i in original
                    select VisitMemberBinding(i);
                return list.ToArray();
            }

            private new RLinq.MemberBinding VisitMemberBinding(MemberBinding binding)
            {
                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return VisitMemberAssignment((MemberAssignment)binding);

                    case MemberBindingType.MemberBinding:
                        return VisitMemberMemberBinding((MemberMemberBinding)binding);

                    case MemberBindingType.ListBinding:
                        return VisitMemberListBinding((MemberListBinding)binding);

                    default:
                        throw new Exception($"Unhandled binding type '{binding.BindingType}'");
                }
            }

            private new RLinq.MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                var e = Visit(assignment.Expression).Unwrap();
                var m = Aqua.TypeSystem.MemberInfo.Create(assignment.Member);
                return new RLinq.MemberAssignment(m, e);
            }

            private new RLinq.MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = Aqua.TypeSystem.MemberInfo.Create(binding.Member);
                return new RLinq.MemberMemberBinding(m, bindings);
            }

            private new RLinq.MemberListBinding VisitMemberListBinding(MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = Aqua.TypeSystem.MemberInfo.Create(binding.Member);
                return new RLinq.MemberListBinding(m, initializers);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var instance = Visit(node.Object).Unwrap();
                var arguments =
                    from argument in node.Arguments
                    select Visit(argument).Unwrap();
                return new RLinq.MethodCallExpression(instance, node.Method, arguments).Wrap();
            }

            protected override Expression VisitLambda(LambdaExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var parameters =
                    from p in node.Parameters
                    select (RLinq.ParameterExpression)VisitParameter(p).Unwrap();
                return new RLinq.LambdaExpression(node.Type, body, parameters).Wrap();
            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                var unaryOperator = node.NodeType.ToUnaryOperator();
                var operand = Visit(node.Operand).Unwrap();
                return new RLinq.UnaryExpression(unaryOperator, operand, node.Type, node.Method).Wrap();
            }

            protected override Expression VisitConditional(ConditionalExpression node)
            {
                var test = Visit(node.Test).Unwrap();
                var ifTrue = Visit(node.IfTrue).Unwrap();
                var ifFalse = Visit(node.IfFalse).Unwrap();
                return new RLinq.ConditionalExpression(test, ifTrue, ifFalse).Wrap();
            }

            protected override Expression VisitNewArray(NewArrayExpression node)
            {
                var newArrayType = node.NodeType.ToNewArrayType();
                var expressions = VisitExpressionList(node.Expressions);
                var rlinqExpressions =
                    from i in expressions
                    select i.Unwrap();
                var elementType = TypeHelper.GetElementType(node.Type);
                return new RLinq.NewArrayExpression(newArrayType, elementType, rlinqExpressions).Wrap();
            }
            
            protected override Expression VisitBlock(BlockExpression node)
            {
                var expressions = VisitExpressionList(node.Expressions);
                var rlinqExpressions = 
                    from exp in expressions
                    select exp.Unwrap();

                IEnumerable<RLinq.ParameterExpression> rlinqVariables = null;
                if (!ReferenceEquals(null, node.Variables))
                {
                    var nodeVariables = node.Variables.Cast<Expression>().ToList().AsReadOnly();
                    IEnumerable<Expression> variables = VisitExpressionList(nodeVariables);
                    rlinqVariables =
                        from exp in variables
                        select (RLinq.ParameterExpression)exp.Unwrap();
                }
                
                var type = node.Type == node.Result.Type ? null : node.Type;
                return new RLinq.BlockExpression(type, rlinqVariables, rlinqExpressions).Wrap();
            }

            protected override Expression VisitDefault(DefaultExpression node)
            {
                return new RLinq.DefaultExpression(node.Type).Wrap();
            }

            protected override Expression VisitLabel(LabelExpression node)
            {
                var target = VisitTarget(node.Target);
                var defaultValue = ReferenceEquals(null, node.DefaultValue) ? null : Visit(node.DefaultValue).Unwrap();
                return new RLinq.LabelExpression(target, defaultValue).Wrap();
            }

            protected override Expression VisitLoop(LoopExpression node)
            {
                var body = Visit(node.Body).Unwrap();
                var breakLabel = VisitTarget(node.BreakLabel);
                var continueLabel = VisitTarget(node.ContinueLabel);
                return new RLinq.LoopExpression(body, breakLabel, continueLabel).Wrap();
            }

            protected override Expression VisitGoto(GotoExpression node)
            {
                var kind = node.Kind.ToGotoExpressionKind();
                var target = VisitTarget(node.Target);
                var type = node.Target.Type == node.Type ? null : node.Type;
                var value = Visit(node.Value).Unwrap();
                return new RLinq.GotoExpression(kind, target, type, value).Wrap();
            }

            private RLinq.LabelTarget VisitTarget(LabelTarget labelTarget)
            {
                if (ReferenceEquals(null, labelTarget))
                {
                    return null;
                }

                RLinq.LabelTarget target;
                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out target))
                    {
                        target = new RLinq.LabelTarget(labelTarget.Name, labelTarget.Type, _labelTargetCache.Count + 1);
                        _labelTargetCache.Add(labelTarget, target);
                    }
                }

                return target;
            }

            private static NotSupportedException CreateNotSupportedException(Expression expression)
            {
                return new NotSupportedException($"expression: '{expression}'");
            }
        }

        private sealed class RemoteExpressionToLinqExpressionTranslator : IEqualityComparer<RLinq.ParameterExpression>, IEqualityComparer<RLinq.LabelTarget>
        {
            private readonly Dictionary<RLinq.ParameterExpression, ParameterExpression> _parameterExpressionCache;
            private readonly Dictionary<RLinq.LabelTarget, LabelTarget> _labelTargetCache;
            private readonly ITypeResolver _typeResolver;

            public RemoteExpressionToLinqExpressionTranslator(ITypeResolver typeResolver)
            {
                _parameterExpressionCache = new Dictionary<RLinq.ParameterExpression, ParameterExpression>(this);
                _labelTargetCache = new Dictionary<RLinq.LabelTarget, LabelTarget>(this);
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            public Expression ToExpression(RLinq.Expression expression)
            {
                var exp = Visit(expression);
                return exp;
            }

            private Expression Visit(RLinq.Expression expression)
            {
                if (ReferenceEquals(null, expression))
                {
                    return null;
                }

                switch (expression.NodeType)
                {
                    case RLinq.ExpressionType.Binary:
                        return VisitBinary((RLinq.BinaryExpression)expression);

                    case RLinq.ExpressionType.Block:
                        return VisitBlock((RLinq.BlockExpression)expression);

                    case RLinq.ExpressionType.Call:
                        return VisitMethodCall((RLinq.MethodCallExpression)expression);

                    case RLinq.ExpressionType.Conditional:
                        return VisitConditional((RLinq.ConditionalExpression)expression);

                    case RLinq.ExpressionType.Constant:
                        return VisitConstant((RLinq.ConstantExpression)expression);

                    case RLinq.ExpressionType.Default:
                        return VisitDefault((RLinq.DefaultExpression)expression);

                    case RLinq.ExpressionType.Goto:
                        return VisitGoto((RLinq.GotoExpression)expression);

                    case RLinq.ExpressionType.Label:
                        return VisitLabel((RLinq.LabelExpression)expression);

                    case RLinq.ExpressionType.Lambda:
                        return VisitLambda((RLinq.LambdaExpression)expression);

                    case RLinq.ExpressionType.ListInit:
                        return VisitListInit((RLinq.ListInitExpression)expression);

                    case RLinq.ExpressionType.Loop:
                        return VisitLoop((RLinq.LoopExpression)expression);

                    case RLinq.ExpressionType.MemberAccess:
                        return VisitMember((RLinq.MemberExpression)expression);

                    case RLinq.ExpressionType.MemberInit:
                        return VisitMemberInit((RLinq.MemberInitExpression)expression);

                    case RLinq.ExpressionType.New:
                        return VisitNew((RLinq.NewExpression)expression);

                    case RLinq.ExpressionType.NewArray:
                        return VisitNewArray((RLinq.NewArrayExpression)expression);

                    case RLinq.ExpressionType.Parameter:
                        return VisitParameter((RLinq.ParameterExpression)expression);

                    case RLinq.ExpressionType.Unary:
                        return VisitUnary((RLinq.UnaryExpression)expression);

                    case RLinq.ExpressionType.Try:
                        return VisitTry((RLinq.TryExpression)expression);

                    case RLinq.ExpressionType.TypeIs:
                        return VisitTypeIs((RLinq.TypeBinaryExpression)expression);

                    default:
                        throw new Exception(string.Format("Unknown expression note type: '{0}'", expression.NodeType));
                }
            }

            private Expression VisitTry(RLinq.TryExpression tryExpression)
            {
                Expression body = Visit(tryExpression.Body);
                Type type = ReferenceEquals(tryExpression.Type, null) ? null : _typeResolver.ResolveType(tryExpression.Type);
                Expression fault = ReferenceEquals(tryExpression.Fault, null) ? null : Visit(tryExpression.Fault);
                Expression @finally = ReferenceEquals(tryExpression.Finally, null) ? null : Visit(tryExpression.Finally);
                IEnumerable<CatchBlock> handlers = tryExpression.Handlers?.Select(VisitCatchBlock) ?? Enumerable.Empty<CatchBlock>();

                return Expression.MakeTry(type, body, @finally, fault,handlers);
            }

            private CatchBlock VisitCatchBlock(RLinq.CatchBlock catchBlock)
            {
                Type exceptionType = ReferenceEquals(catchBlock.Test, null) ? null : _typeResolver.ResolveType(catchBlock.Test);
                ParameterExpression exceptionParameter = ReferenceEquals(catchBlock.Variable, null) ? null : VisitParameter(catchBlock.Variable);
                Expression body = ReferenceEquals(catchBlock.Body, null) ? null : Visit(catchBlock.Body);
                Expression filter = ReferenceEquals(catchBlock.Filter,null) ? null : Visit(catchBlock.Filter);

                return Expression.MakeCatchBlock(exceptionType, exceptionParameter, body,filter);
            }

            private NewExpression VisitNew(RLinq.NewExpression newExpression)
            {
                var constructor = newExpression.Constructor.ResolveConstructor(_typeResolver);
                if (ReferenceEquals(null, newExpression.Arguments))
                {
                    if (newExpression.Members?.Any() ?? false)
                    {
                        var members = newExpression.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                        return Expression.New(constructor, new Expression[0], members);
                    }
                    else
                    {
                        return Expression.New(constructor);
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
                        return Expression.New(constructor, arguments, members);
                    }
                    else
                    {
                        return Expression.New(constructor, arguments);
                    }
                }
            }

            private Expression VisitNewArray(RLinq.NewArrayExpression expression)
            {
                var expressions = VisitExpressionList(expression.Expressions);
                var type = _typeResolver.ResolveType(expression.Type);
                switch (expression.NewArrayType)
                {
                    case RLinq.NewArrayType.NewArrayBounds:
                        return Expression.NewArrayBounds(type, expressions);

                    case RLinq.NewArrayType.NewArrayInit:
                        return Expression.NewArrayInit(type, expressions);

                    default:
                        throw new Exception($"Unhandled new array type {expression.NewArrayType}");
                }
            }

            private Expression VisitMemberInit(RLinq.MemberInitExpression expression)
            {
                var n = VisitNew(expression.NewExpression);
                var bindings = VisitBindingList(expression.Bindings);
                return Expression.MemberInit(n, bindings);
            }

            private Expression VisitBlock(RLinq.BlockExpression node)
            {
                var type = node.Type.ResolveType(_typeResolver);

                var variables =
                    from i in node.Variables ?? Enumerable.Empty<RLinq.ParameterExpression>()
                    select (ParameterExpression)Visit(i);

                var expressions =
                    from i in node.Expressions ?? Enumerable.Empty<RLinq.Expression>()
                    select Visit(i);

                return ReferenceEquals(null, type)
                    ? Expression.Block(variables, expressions)
                    : Expression.Block(type, variables, expressions);
            }

            private IEnumerable<MemberBinding> VisitBindingList(IEnumerable<RLinq.MemberBinding> original)
            {
                var list =
                    from i in original
                    select VisitMemberBinding(i);
                return list.ToArray();
            }

            private MemberBinding VisitMemberBinding(RLinq.MemberBinding binding)
            {
                switch (binding.BindingType)
                {
                    case RLinq.MemberBindingType.Assignment:
                        return VisitMemberAssignment((RLinq.MemberAssignment)binding);

                    case RLinq.MemberBindingType.MemberBinding:
                        return VisitMemberMemberBinding((RLinq.MemberMemberBinding)binding);

                    case RLinq.MemberBindingType.ListBinding:
                        return VisitMemberListBinding((RLinq.MemberListBinding)binding);

                    default:
                        throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
                }
            }

            private MemberAssignment VisitMemberAssignment(RLinq.MemberAssignment assignment)
            {
                var e = Visit(assignment.Expression);
                var m = assignment.Member.ResolveMemberInfo(_typeResolver);
                return Expression.Bind(m, e);
            }

            private MemberMemberBinding VisitMemberMemberBinding(RLinq.MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = binding.Member.ResolveMemberInfo(_typeResolver);
                return Expression.MemberBind(m, bindings);
            }

            private MemberListBinding VisitMemberListBinding(RLinq.MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = binding.Member.ResolveMemberInfo(_typeResolver);
                return Expression.ListBind(m, initializers);
            }

            private IEnumerable<ElementInit> VisitElementInitializerList(IEnumerable<RLinq.ElementInit> original)
            {
                var list =
                    from i in original
                    select VisitElementInitializer(i);
                return list.ToArray();
            }

            private ElementInit VisitElementInitializer(RLinq.ElementInit initializer)
            {
                var arguments = VisitExpressionList(initializer.Arguments);
                var m = initializer.AddMethod.ResolveMethod(_typeResolver);
                return Expression.ElementInit(m, arguments);
            }

            private IEnumerable<Expression> VisitExpressionList(IEnumerable<RLinq.Expression> original)
            {
                var list =
                    from i in original
                    select Visit(i);
                return list.ToArray();
            }

            private Expression VisitListInit(RLinq.ListInitExpression listInitExpression)
            {
                var n = VisitNew(listInitExpression.NewExpression);
                var initializers =
                    from i in listInitExpression.Initializers
                    select Expression.ElementInit(i.AddMethod.ResolveMethod(_typeResolver), i.Arguments.Select(a => Visit(a)));
                return Expression.ListInit(n, initializers);
            }

            private ParameterExpression VisitParameter(RLinq.ParameterExpression parameterExpression)
            {
                ParameterExpression exp;
                lock (_parameterExpressionCache)
                {
                    if (!_parameterExpressionCache.TryGetValue(parameterExpression, out exp))
                    {
                        var type = _typeResolver.ResolveType(parameterExpression.ParameterType);
                        exp = Expression.Parameter(type, parameterExpression.ParameterName);
                        _parameterExpressionCache.Add(parameterExpression, exp);
                    }
                }

                return exp;
            }

            private Expression VisitUnary(RLinq.UnaryExpression unaryExpression)
            {
                var expressionType = unaryExpression.UnaryOperator.ToExpressionType();
                var exp = Visit(unaryExpression.Operand);
                var type = ReferenceEquals(null, unaryExpression.Type) ? null : _typeResolver.ResolveType(unaryExpression.Type);
                var method = unaryExpression.Method?.ResolveMethod(_typeResolver);
                return Expression.MakeUnary(expressionType, exp, type, method);
            }

            private Expression VisitMember(RLinq.MemberExpression memberExpression)
            {
                var exp = Visit(memberExpression.Expression);
                var m = memberExpression.Member.ResolveMemberInfo(_typeResolver);
                return Expression.MakeMemberAccess(exp, m);
            }

            private Expression VisitMethodCall(RLinq.MethodCallExpression methodCallExpression)
            {
                var instance = Visit(methodCallExpression.Instance);
                var arguments = methodCallExpression.Arguments
                    .Select(x => Visit(x))
                    .ToArray();                
                var methodInfo = methodCallExpression.Method.ResolveMethod(_typeResolver);
                return Expression.Call(instance, methodInfo, arguments);
            }

            private Expression VisitConditional(RLinq.ConditionalExpression expression)
            {
                var test = Visit(expression.Test);
                var ifTrue = Visit(expression.IfTrue);
                var ifFalse = Visit(expression.IfFalse);

                if (ifFalse is DefaultExpression && ifFalse.Type == typeof(void))
                {
                    return Expression.IfThen(test, ifTrue);
                }

                return Expression.Condition(test, ifTrue, ifFalse);
            }

            private Expression VisitConstant(RLinq.ConstantExpression constantValueExpression)
            {
                var value = constantValueExpression.Value;
                var type = constantValueExpression.Type.ResolveType(_typeResolver);

                var oldConstantQueryArgument = value as ConstantQueryArgument;
                if (!ReferenceEquals(null, oldConstantQueryArgument?.Type))
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(oldConstantQueryArgument.Type);
                    foreach (var property in oldConstantQueryArgument.Properties)
                    {
                        var propertyValue = property.Value;
                        var expressionValue = propertyValue as RLinq.Expression;
                        if (!ReferenceEquals(null, expressionValue))
                        {
                            propertyValue = Visit(expressionValue);
                        }

                        newConstantQueryArgument.Add(property.Name, propertyValue);
                    }

                    var argumentType = newConstantQueryArgument.Type.ResolveType(_typeResolver);
                    value = ConstantValueMapper.ForReconstruction(_typeResolver).Map(newConstantQueryArgument, argumentType);

                    if (ReferenceEquals(null, type) || !type.IsAssignableFrom(argumentType))
                    {
                        type = argumentType;
                    }
                }

                return Expression.Constant(value, type);
            }

            private Expression VisitBinary(RLinq.BinaryExpression binaryExpression)
            {
                var p1 = Visit(binaryExpression.LeftOperand);
                var p2 = Visit(binaryExpression.RightOperand);
                var conversion = Visit(binaryExpression.Conversion) as LambdaExpression;
                var binaryType = binaryExpression.BinaryOperator.ToExpressionType();
                var method = ReferenceEquals(null, binaryExpression.Method) ? null : binaryExpression.Method.ResolveMethod(_typeResolver);
                return Expression.MakeBinary(binaryType, p1, p2, binaryExpression.IsLiftedToNull, method, conversion);
            }

            private Expression VisitTypeIs(RLinq.TypeBinaryExpression typeBinaryExpression)
            {
                var expression = Visit(typeBinaryExpression.Expression);
                var type = _typeResolver.ResolveType(typeBinaryExpression.TypeOperand);
                return Expression.TypeIs(expression, type);
            }

            private Expression VisitLambda(RLinq.LambdaExpression lambdaExpression)
            {
                var body = Visit(lambdaExpression.Expression);
                var parameters =
                    from p in lambdaExpression.Parameters
                    select VisitParameter(p);

                if (ReferenceEquals(null, lambdaExpression.Type))
                {
                    return Expression.Lambda(body, parameters.ToArray());
                }
                else
                {
                    var delegateType = _typeResolver.ResolveType(lambdaExpression.Type);
                    return Expression.Lambda(delegateType, body, parameters.ToArray());
                }
            }

            private Expression VisitDefault(RLinq.DefaultExpression defaultExpression)
            {
                var type = defaultExpression.Type.ResolveType(_typeResolver);
                return Expression.Default(type);
            }

            private Expression VisitGoto(RLinq.GotoExpression gotoExpression)
            {
                var kind = gotoExpression.Kind.ToGotoExpressionKind();
                var target = VisitTarget(gotoExpression.Target);
                var value = Visit(gotoExpression.Value);
                var type = gotoExpression.Type.ResolveType(_typeResolver);
                return Expression.MakeGoto(kind, target, value, type ?? target.Type);
            }

            private Expression VisitLabel(RLinq.LabelExpression labelExpression)
            {
                var target = VisitTarget(labelExpression.Target);
                var defaultValue = Visit(labelExpression.DefaultValue);
                return Expression.Label(target, defaultValue);
            }

            private Expression VisitLoop(RLinq.LoopExpression loopExpression)
            {
                var body = Visit(loopExpression.Body);
                var breakLabel = VisitTarget(loopExpression.BreakLabel);
                var continueLabel = VisitTarget(loopExpression.ContinueLabel);
                return Expression.Loop(body, breakLabel, continueLabel);
            }

            private LabelTarget VisitTarget(RLinq.LabelTarget labelTarget)
            {
                if (ReferenceEquals(null, labelTarget))
                {
                    return null;
                }

                LabelTarget target;
                lock (_labelTargetCache)
                {
                    if (!_labelTargetCache.TryGetValue(labelTarget, out target))
                    {
                        var targetType = labelTarget.Type.ResolveType(_typeResolver);
                        target = Expression.Label(targetType, labelTarget.Name);
                        _labelTargetCache.Add(labelTarget, target);
                    }
                }

                return target;
            }

            bool IEqualityComparer<RLinq.ParameterExpression>.Equals(RLinq.ParameterExpression x, RLinq.ParameterExpression y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(null, x))
                {
                    if (ReferenceEquals(null, y))
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

                if (ReferenceEquals(null, x))
                {
                    if (ReferenceEquals(null, y))
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
