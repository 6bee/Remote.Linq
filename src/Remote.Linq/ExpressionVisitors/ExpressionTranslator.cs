// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
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
        private sealed class ParameterCache
        {
            private readonly Dictionary<object, ParameterExpression> _cache = new Dictionary<object, ParameterExpression>();

            // note: the central parameter repository is required since parameters within an expression tree must be represented by the same parameter expression insatnce
            public ParameterExpression GetParameterExpression(Type type, string name = "$")
            {
                lock (_cache)
                {
                    var key = new { type, name };
                    ParameterExpression exp;
                    if (!_cache.TryGetValue(key, out exp))
                    {
                        exp = Expression.Parameter(type, name);
                        _cache.Add(key, exp);
                    }

                    return exp;
                }
            }

            public IEnumerable<ParameterExpression> GetAllParameters()
            {
                return _cache.Values.ToList();
            }
        }

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
            return new LinqExpressionToRemoteExpressionTranslator().ToRemoteExpression(expression);
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
            var exp = new RemoteExpressionToLinqExpressionTranslator(new ParameterCache(), typeResolver).ToExpression(expression);
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
            var parameterCache = new ParameterCache();
            var lambdaExpression = new RemoteExpressionToLinqExpressionTranslator(parameterCache, typeResolver).ToExpression(expression);
            return (LambdaExpression)lambdaExpression;
        }

        private static ExpressionType ToExpressionType(this RLinq.BinaryOperator binaryOperator) => (ExpressionType)(int)binaryOperator;

        private static ExpressionType ToExpressionType(this RLinq.UnaryOperator unaryOperator) => (ExpressionType)(int)unaryOperator;

        private static RLinq.BinaryOperator ToBinaryOperator(this ExpressionType expressionType) => (RLinq.BinaryOperator)(int)expressionType;

        private static RLinq.UnaryOperator ToUnaryOperator(this ExpressionType expressionType) => (RLinq.UnaryOperator)(int)expressionType;

        private static RLinq.NewArrayType ToNewArrayType(this ExpressionType expressionType) => (RLinq.NewArrayType)(int)expressionType;
        
        private static ConstantExpression Wrap(this RLinq.Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : Expression.Constant(expression);
        }

        private static RLinq.ConstantExpression Wrap(this Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : RLinq.Expression.Constant(expression);
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

        private static bool KeepMarkerFuncitons(Expression expression)
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

        private sealed class LinqExpressionToRemoteExpressionTranslator : ExpressionVisitorBase
        {
            public RLinq.Expression ToRemoteExpression(Expression expression)
            {
                var partialEvalExpression = expression.PartialEval(KeepMarkerFuncitons);
                if (partialEvalExpression == null)
                {
                    throw CreateNotSupportedException(expression);
                }

                var constExpression = Visit(partialEvalExpression);
                return constExpression.Unwrap();
            }

            public RLinq.LambdaExpression ToRemoteExpression(LambdaExpression expression)
            {
                var partialEvalExpression = expression.PartialEval(KeepMarkerFuncitons) as LambdaExpression;
                if (partialEvalExpression == null)
                {
                    throw CreateNotSupportedException(expression);
                }

                var constExpression = Visit(partialEvalExpression.Body);
                var parameters =
                    from p in partialEvalExpression.Parameters
                    select new RLinq.ParameterExpression(p.Name, p.Type);
                return new RLinq.LambdaExpression(constExpression.Unwrap(), parameters);
            }

            protected override Expression Visit(Expression exp)
            {
                if (ReferenceEquals(null, exp))
                {
                    return exp;
                }

                switch (exp.NodeType)
                {
                    case ExpressionType.New:
                        return VisitNew((NewExpression)exp).Wrap();

                    default:
                        return base.Visit(exp);
                }
            }

            protected override Expression VisitListInit(ListInitExpression init)
            {
                var n = VisitNew(init.NewExpression);
                var initializers = VisitElementInitializerList(init.Initializers);
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

            private new RLinq.NewExpression VisitNew(NewExpression nex)
            {
                var arguments = default(IEnumerable<RLinq.Expression>);
                if (nex.Arguments != null && nex.Arguments.Count > 0)
                {
                    arguments =
                        from arg in nex.Arguments
                        select Visit(arg).Unwrap();
                }

                return new RLinq.NewExpression(nex.Constructor, arguments, nex.Members);
            }

            protected override Expression VisitConstant(ConstantExpression c)
            {
                return new RLinq.ConstantExpression(c.Value, c.Type).Wrap();
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                return new RLinq.ParameterExpression(p.Name, p.Type).Wrap();
            }

            protected override Expression VisitBinary(BinaryExpression b)
            {
                var binaryOperator = b.NodeType.ToBinaryOperator();
                var left = Visit(b.Left).Unwrap();
                var right = Visit(b.Right).Unwrap();
                var conversion = Visit(b.Conversion).Unwrap() as RLinq.LambdaExpression;
                return new RLinq.BinaryExpression(binaryOperator, left, right, b.IsLiftedToNull, b.Method, conversion).Wrap();
            }

            protected override Expression VisitTypeIs(TypeBinaryExpression b)
            {
                var expression = Visit(b.Expression).Unwrap();
                return new RLinq.TypeBinaryExpression(expression, b.TypeOperand).Wrap();
            }

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                var instance = Visit(m.Expression).Unwrap();
                return new RLinq.MemberExpression(instance, m.Member).Wrap();
            }

            protected override Expression VisitMemberInit(MemberInitExpression init)
            {
                var n = VisitNew(init.NewExpression);
                var bindings = VisitBindingList(init.Bindings);
                return RLinq.Expression.MemberInit(n, bindings).Wrap();
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

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                var instance = Visit(m.Object).Unwrap();
                var arguments =
                    from argument in m.Arguments
                    select Visit(argument).Unwrap();
                return new RLinq.MethodCallExpression(instance, m.Method, arguments).Wrap();
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                var body = Visit(lambda.Body).Unwrap();
                var parameters =
                    from p in lambda.Parameters
                    select (RLinq.ParameterExpression)VisitParameter(p).Unwrap();
                return new RLinq.LambdaExpression(lambda.Type, body, parameters).Wrap();
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                var unaryOperator = u.NodeType.ToUnaryOperator();
                var operand = Visit(u.Operand).Unwrap();
                return new RLinq.UnaryExpression(unaryOperator, operand, u.Type, u.Method).Wrap();
            }

            protected override Expression VisitConditional(ConditionalExpression c)
            {
                var test = Visit(c.Test).Unwrap();
                var ifTrue = Visit(c.IfTrue).Unwrap();
                var ifFalse = Visit(c.IfFalse).Unwrap();
                return new RLinq.ConditionalExpression(test, ifTrue, ifFalse).Wrap();
            }

            protected override Expression VisitNewArray(NewArrayExpression na)
            {
                var newArrayType = na.NodeType.ToNewArrayType();
                var expressions = VisitExpressionList(na.Expressions);
                var rlinqExpressions =
                    from i in expressions
                    select i.Unwrap();
                var elementType = TypeHelper.GetElementType(na.Type);
                return new RLinq.NewArrayExpression(newArrayType, elementType, rlinqExpressions).Wrap();
            }

            private static NotSupportedException CreateNotSupportedException(Expression expression)
            {
                return new NotSupportedException($"expression: '{expression}'");
            }
        }

        private sealed class RemoteExpressionToLinqExpressionTranslator
        {
            private readonly ParameterCache _parameterCache;
            private readonly ITypeResolver _typeResolver;

            public RemoteExpressionToLinqExpressionTranslator(ParameterCache parameterCache, ITypeResolver typeResolver)
            {
                _parameterCache = parameterCache;
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

                    case RLinq.ExpressionType.Conditional:
                        return VisitConditional((RLinq.ConditionalExpression)expression);

                    case RLinq.ExpressionType.Constant:
                        return VisitConstant((RLinq.ConstantExpression)expression);

                    case RLinq.ExpressionType.Parameter:
                        return VisitParameter((RLinq.ParameterExpression)expression);

                    case RLinq.ExpressionType.MemberAccess:
                        return VisitMember((RLinq.MemberExpression)expression);

                    case RLinq.ExpressionType.Unary:
                        return VisitUnary((RLinq.UnaryExpression)expression);

                    case RLinq.ExpressionType.Call:
                        return VisitMethodCall((RLinq.MethodCallExpression)expression);

                    case RLinq.ExpressionType.Lambda:
                        return VisitLambda((RLinq.LambdaExpression)expression);

                    case RLinq.ExpressionType.ListInit:
                        return VisitListInit((RLinq.ListInitExpression)expression);

                    case RLinq.ExpressionType.New:
                        return VisitNew((RLinq.NewExpression)expression);

                    case RLinq.ExpressionType.NewArray:
                        return VisitNewArray((RLinq.NewArrayExpression)expression);

                    case RLinq.ExpressionType.MemberInit:
                        return VisitMemberInit((RLinq.MemberInitExpression)expression);

                    case RLinq.ExpressionType.TypeIs:
                        return VisitTypeIs((RLinq.TypeBinaryExpression)expression);

                    default:
                        throw new Exception(string.Format("Unknown expression note type: '{0}'", expression.NodeType));
                }
            }

            private NewExpression VisitNew(RLinq.NewExpression newExpression)
            {
                var constructor = newExpression.Constructor.ResolveConstructor(_typeResolver);
                if (newExpression.Arguments != null)
                {
                    var arguments =
                        from a in newExpression.Arguments
                        select Visit(a);
                    if (ReferenceEquals(null, newExpression.Members))
                    {
                        return Expression.New(constructor, arguments);
                    }
                    else
                    {
                        var members = newExpression.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                        return Expression.New(constructor, arguments, members);
                    }
                }
                else
                {
                    if (ReferenceEquals(null, newExpression.Members))
                    {
                        return Expression.New(constructor);
                    }
                    else
                    {
                        var members = newExpression.Members.Select(x => x.ResolveMemberInfo(_typeResolver)).ToArray();
                        return Expression.New(constructor, new Expression[0], members);
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
                var parameterType = _typeResolver.ResolveType(parameterExpression.ParameterType);
                return _parameterCache.GetParameterExpression(parameterType, parameterExpression.ParameterName);
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
                return Expression.Condition(test, ifTrue, ifFalse);
            }

            private Expression VisitConstant(RLinq.ConstantExpression constantValueExpression)
            {
                var type = _typeResolver.ResolveType(constantValueExpression.Type);
                return Expression.Constant(constantValueExpression.Value, type);
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
        }
    }
}
