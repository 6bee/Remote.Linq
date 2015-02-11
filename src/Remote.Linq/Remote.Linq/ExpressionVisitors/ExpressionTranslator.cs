// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.ExpressionVisitors;
    using Remote.Linq.TypeSystem;
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

        internal static System.Linq.Expressions.ConstantExpression Wrap(this RLinq.Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : System.Linq.Expressions.Expression.Constant(expression);
        }

        internal static RLinq.ConstantExpression Wrap(this System.Linq.Expressions.Expression expression)
        {
            return ReferenceEquals(null, expression) ? null : RLinq.Expression.Constant(expression);
        }

        internal static RLinq.Expression Unwrap(this System.Linq.Expressions.Expression expression)
        {
            if (!ReferenceEquals(null, expression) && expression.NodeType == ExpressionType.Constant && typeof(RLinq.Expression).IsAssignableFrom(expression.Type))
            {
                return (RLinq.Expression)((System.Linq.Expressions.ConstantExpression)expression).Value;
            }
            else
            {
                return null;
            }
        }

        internal static System.Linq.Expressions.Expression Unwrap(this RLinq.Expression expression)
        {
            if (!ReferenceEquals(null, expression) && expression.NodeType == RLinq.ExpressionType.Constant && ((RLinq.ConstantExpression)expression).Value is System.Linq.Expressions.Expression)
            {
                return (System.Linq.Expressions.Expression)((RLinq.ConstantExpression)expression).Value;
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
                if (ReferenceEquals(null, exp)) return exp;

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
                var left = Visit(b.Left).Unwrap();
                var right = Visit(b.Right).Unwrap();

                if (ReferenceEquals(null, b.Conversion))
                {
                    if (b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual)
                    {
                        if (left is RLinq.ConstantExpression)
                        {
                            if (((RLinq.ConstantExpression)left).Value is bool)
                            {
                                if (b.NodeType == ExpressionType.Equal ^ (bool)((RLinq.ConstantExpression)left).Value)
                                {
                                    // != true
                                    // == false
                                    return new RLinq.UnaryExpression(right, RLinq.UnaryOperator.Not).Wrap();
                                }
                                else
                                {
                                    // == true
                                    // != false
                                    return right.Wrap();
                                }
                            }
                            if (ReferenceEquals(null, ((RLinq.ConstantExpression)left).Value) /* && ReferenceEquals(null, ((Filter.ConstantValueExpression)right).Value)*/)
                            {
                                return new RLinq.UnaryExpression(right, b.NodeType == ExpressionType.Equal ? RLinq.UnaryOperator.IsNull : RLinq.UnaryOperator.IsNotNull).Wrap();
                            }
                        }
                        if (right is RLinq.ConstantExpression)
                        {
                            if (((RLinq.ConstantExpression)right).Value is bool)
                            {
                                if (b.NodeType == ExpressionType.Equal ^ (bool)((RLinq.ConstantExpression)right).Value)
                                {
                                    // != true
                                    // == false
                                    return new RLinq.UnaryExpression(left, RLinq.UnaryOperator.Not).Wrap();
                                }
                                else
                                {
                                    // == true
                                    // != false
                                    return left.Wrap();
                                }
                            }
                            if (ReferenceEquals(null, ((RLinq.ConstantExpression)right).Value) /* && !ReferenceEquals(null, ((Filter.ConstantValueExpression)left).Value)*/)
                            {
                                return new RLinq.UnaryExpression(left, b.NodeType == ExpressionType.Equal ? RLinq.UnaryOperator.IsNull : RLinq.UnaryOperator.IsNotNull).Wrap();
                            }
                        }
                    }
                }

                var conversion = Visit(b.Conversion).Unwrap() as RLinq.LambdaExpression;
                return new RLinq.BinaryExpression(left, right, TranslateBinaryOperator(b.NodeType), b.IsLiftedToNull, b.Method, conversion).Wrap();
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
                        throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
                }
            }

            private new RLinq.MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
            {
                var e = Visit(assignment.Expression).Unwrap();
                var m = TypeSystem.MemberInfo.Create(assignment.Member);
                return new RLinq.MemberAssignment(m, e);
            }

            private new RLinq.MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
            {
                var bindings = VisitBindingList(binding.Bindings);
                var m = TypeSystem.MemberInfo.Create(binding.Member);
                return new RLinq.MemberMemberBinding(m, bindings);
            }

            private new RLinq.MemberListBinding VisitMemberListBinding(MemberListBinding binding)
            {
                var initializers = VisitElementInitializerList(binding.Initializers);
                var m = TypeSystem.MemberInfo.Create(binding.Member);
                return new RLinq.MemberListBinding(m, initializers);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                // property selector
                if (m.Method.Name == "get_Item" && m.Arguments.Count == 1 && m.Arguments[0].NodeType == ExpressionType.Constant && ((ConstantExpression)m.Arguments[0]).Type == typeof(string))
                {
                    var exp = (ConstantExpression)m.Arguments[0];
                    var instance = Visit(m.Object).Unwrap();
                    var propertyInfo = m.Object.Type.GetProperty((string)exp.Value, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    if (propertyInfo == null) throw new Exception(string.Format("'{0}' is not a valid or an ambiguous property of type {1}", (string)exp.Value, m.Object.Type.FullName));
                    return new RLinq.MemberExpression(instance, propertyInfo).Wrap();
                }

                if (m.Object != null && m.Object.Type == typeof(string) && m.Arguments.Count == 1)
                {
                    var obj = Visit(m.Object);
                    var p1 = obj.Unwrap();

                    var param = Visit(m.Arguments[0]);
                    var p2 = param.Unwrap();

                    switch (((MethodCallExpression)m).Method.Name)
                    {
                        case "Contains":
                            return new RLinq.BinaryExpression(p1, p2, RLinq.BinaryOperator.Contains).Wrap();
                        case "StartsWith":
                            return new RLinq.BinaryExpression(p1, p2, RLinq.BinaryOperator.StartsWith).Wrap();
                        case "EndsWith":
                            return new RLinq.BinaryExpression(p1, p2, RLinq.BinaryOperator.EndsWith).Wrap();
                    }
                }

                if (m.Method.Name == "Contains" && m.Object == null && m.Arguments.Count == 2 && m.Arguments[0].NodeType == ExpressionType.Constant && ((ConstantExpression)m.Arguments[0]).Value is System.Collections.IEnumerable)
                {
                    var collection = ((ConstantExpression)m.Arguments[0]).Value;
                    var collectionType = collection.GetType();
                    var elementType = collectionType.GetElementType();
                    if (elementType == null)
                    {
                        var genericArguments = collectionType.GetGenericArguments();
                        if (genericArguments != null && genericArguments.Count() == 1)
                        {
                            elementType = genericArguments.Single();
                        }
                    }
                    if (elementType == null)
                    {
                        throw new Exception(string.Format("Unable to retrieve element type for collection type {0}", collectionType));
                    }

                    var list =
                        from item in ((System.Collections.IEnumerable)collection).OfType<object>()
                        select new RLinq.ConstantExpression(item);

                    var a2 = Visit(m.Arguments[1]).Unwrap();

                    return new RLinq.BinaryExpression(a2, new RLinq.CollectionExpression(list, elementType), RLinq.BinaryOperator.In).Wrap();
                }

                {
                    var instance = m.Object == null ? null : Visit(m.Object).Unwrap();
                    var arguments =
                        from argument in m.Arguments
                        select Visit(argument).Unwrap();

                    return new RLinq.MethodCallExpression(instance, ((MethodCallExpression)m).Method, arguments).Wrap();
                }

                //throw CreateNotSupportedException(m);
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                var body = Visit(lambda.Body).Unwrap();
                var parameters =
                    from p in lambda.Parameters
                    select (RLinq.ParameterExpression)VisitParameter(p).Unwrap();
                return new RLinq.LambdaExpression(body, parameters).Wrap();
            }

            protected override Expression VisitUnary(UnaryExpression u)
            {
                var operand = Visit(u.Operand).Unwrap();

                if (u.NodeType == ExpressionType.Convert && u.Type == u.Type.GetUnderlyingSystemType())
                {
                    return new RLinq.ConversionExpression(operand, u.Type).Wrap();
                }

                if (u.NodeType == ExpressionType.Negate && operand != null)
                {
                    return new RLinq.UnaryExpression(operand, RLinq.UnaryOperator.Negate).Wrap();
                }

                if (u.NodeType == ExpressionType.Not && operand != null)
                {
                    return new RLinq.UnaryExpression(operand, RLinq.UnaryOperator.Not).Wrap();
                }

                if (u.NodeType == ExpressionType.Quote && operand != null)
                {
                    //return operand.Wrap();
                    return new RLinq.UnaryExpression(operand, RLinq.UnaryOperator.Quote).Wrap();
                }

                throw CreateNotSupportedException(u);
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
                var expressions = VisitExpressionList(na.Expressions);
                var rlinqExpressions =
                    from i in expressions
                    select i.Unwrap();
                var elementType = TypeHelper.GetElementType(na.Type);
                return new RLinq.NewArrayExpression(elementType, rlinqExpressions).Wrap();
            }

            private static NotSupportedException CreateNotSupportedException(Expression expression)
            {
                return new NotSupportedException(string.Format("expression: '{0}'", expression));
            }

            private static RLinq.BinaryOperator TranslateBinaryOperator(ExpressionType nodeType)
            {
                switch (nodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        return RLinq.BinaryOperator.Add;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        return RLinq.BinaryOperator.Subtract;
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                        return RLinq.BinaryOperator.Multiply;
                    case ExpressionType.Divide:
                        return RLinq.BinaryOperator.Divide;
                    case ExpressionType.Modulo:
                        return RLinq.BinaryOperator.Modulo;
                    case ExpressionType.And:
                        return RLinq.BinaryOperator.BitwiseAnd;
                    case ExpressionType.AndAlso:
                        return RLinq.BinaryOperator.And;
                    case ExpressionType.Or:
                        return RLinq.BinaryOperator.BitwiseOr;
                    case ExpressionType.OrElse:
                        return RLinq.BinaryOperator.Or;
                    case ExpressionType.LessThan:
                        return RLinq.BinaryOperator.LessThan;
                    case ExpressionType.LessThanOrEqual:
                        return RLinq.BinaryOperator.LessThanOrEqual;
                    case ExpressionType.GreaterThan:
                        return RLinq.BinaryOperator.GreaterThan;
                    case ExpressionType.GreaterThanOrEqual:
                        return RLinq.BinaryOperator.GreaterThanOrEqual;
                    case ExpressionType.Equal:
                        return RLinq.BinaryOperator.Equal;
                    case ExpressionType.NotEqual:
                        return RLinq.BinaryOperator.NotEqual;
                    //case ExpressionType.RightShift:
                    //    return Filter.BinaryOperator.RightShift;
                    //case ExpressionType.LeftShift:
                    //    return Filter.BinaryOperator.LeftShift;
                    case ExpressionType.ExclusiveOr:
                        return RLinq.BinaryOperator.ExclusiveOr;
                    case ExpressionType.Coalesce:
                        return RLinq.BinaryOperator.Coalesce;
                    //case ExpressionType.ArrayIndex:
                    default: throw new NotSupportedException(string.Format("No translation defined for binary operator {0}", nodeType));
                }
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
                if (ReferenceEquals(null, expression)) return null;

                switch (expression.NodeType)
                {
                    case RLinq.ExpressionType.Binary:
                        return VisitBinary((RLinq.BinaryExpression)expression);
                    case RLinq.ExpressionType.Collection:
                        return VisitCollection((RLinq.CollectionExpression)expression);
                    case RLinq.ExpressionType.Conditional:
                        return VisitConditional((RLinq.ConditionalExpression)expression);
                    case RLinq.ExpressionType.Constant:
                        return VisitConstant((RLinq.ConstantExpression)expression);
                    case RLinq.ExpressionType.Conversion:
                        return VisitConversion((RLinq.ConversionExpression)expression);
                    case RLinq.ExpressionType.Parameter:
                        return VisitParameter((RLinq.ParameterExpression)expression);
                    case RLinq.ExpressionType.Member:
                        return VisitMember((RLinq.MemberExpression)expression);
                    case RLinq.ExpressionType.Unary:
                        return VisitUnary((RLinq.UnaryExpression)expression);
                    case RLinq.ExpressionType.MethodCall:
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
                return Expression.NewArrayInit(type, expressions);
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
                var exp = Visit(unaryExpression.Operand);
                switch (unaryExpression.Operator)
                {
                    case RLinq.UnaryOperator.IsNotNull:
                        return Expression.MakeBinary(ExpressionType.NotEqual, exp, Expression.Constant(null));
                    case RLinq.UnaryOperator.IsNull:
                        return Expression.MakeBinary(ExpressionType.Equal, exp, Expression.Constant(null));
                    case RLinq.UnaryOperator.Negate:
                        return Expression.MakeUnary(ExpressionType.Negate, exp, null);
                    case RLinq.UnaryOperator.Not:
                        return Expression.MakeUnary(ExpressionType.Not, exp, typeof(bool));
                    case RLinq.UnaryOperator.Quote:
                        return Expression.MakeUnary(ExpressionType.Quote, exp, null);
                    default:
                        throw new Exception(string.Format("Unknown unary operation: '{0}'", unaryExpression.Operator));
                }
            }

            private Expression VisitMember(RLinq.MemberExpression memberExpression)
            {
                var exp = Visit(memberExpression.Expression);
                var m = memberExpression.Member.ResolveMemberInfo(_typeResolver);
                return Expression.MakeMemberAccess(exp, m);
            }

            private Expression VisitMethodCall(RLinq.MethodCallExpression methodCallExpression)
            {
                var instance = methodCallExpression.Instance == null ? null : Visit(methodCallExpression.Instance);
                var arguments = methodCallExpression.Arguments
                    .Select(x => Visit(x))
                    .ToArray();

                var method = methodCallExpression.Method;
                var declaringType = _typeResolver.ResolveType(method.DeclaringType);
                if (string.Compare(method.Name, "SelectMany") == 0 &&
                    declaringType == typeof(System.Linq.Queryable) &&
                    method.IsGenericMethod &&
                    method.GenericArgumentTypes.Count == method.ParameterTypes.Count) // covers two and three parameters
                {
                    // unwrap lambda expression
                    var argumentArray = arguments;
                    var exp = argumentArray[1];

                    LambdaExpression lambda;
                    if (exp is UnaryExpression)
                    {
                        lambda = (LambdaExpression)((UnaryExpression)exp).Operand;
                    }
                    else if (exp is ConstantExpression)
                    {
                        lambda = (LambdaExpression)((ConstantExpression)exp).Value;
                    }
                    else
                    {
                        throw new NotImplementedException(string.Format("missing implemntation for expression type ({0})  {1}", exp.NodeType, exp.GetType()));
                    }

                    // fixup static delegate type: from any generic collection type to IEnumerable<> to match method signature of SelectMany
                    var delegateTypes = lambda.Type.GetGenericArguments().ToArray();
                    if (delegateTypes.Length == 2)
                    {
                        var collectionType = delegateTypes[1];
                        if (collectionType.IsGenericType() &&
                            collectionType.GetGenericTypeDefinition() != typeof(IEnumerable<>) &&
                            collectionType.GetInterfaces().Any(x => x.IsGenericType() && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                        {
                            var sourceElementType = delegateTypes[0];
                            var elementType = collectionType.GetGenericArguments().First();
                            var delegateType = typeof(Func<,>).MakeGenericType(sourceElementType, typeof(IEnumerable<>).MakeGenericType(elementType));
                            var lambda2 = Expression.Lambda(delegateType, lambda.Body, lambda.Parameters.ToArray());
                            lambda = lambda2;
                        }
                    }

                    if (method.GenericArgumentTypes.Count == 2)
                    {
                        arguments = new[] { argumentArray[0], lambda };
                    }
                    else if (method.GenericArgumentTypes.Count == 3)
                    {
                        arguments = new[] { argumentArray[0], lambda, argumentArray[2] };
                    }
                    else
                    {
                        throw new Exception("unexpected method");
                    }
                }

                var methodInfo = method.ResolveMethod(_typeResolver);
                return Expression.Call(instance, methodInfo, arguments);
            }

            private Expression VisitConversion(RLinq.ConversionExpression conversionExpression)
            {
                var exp = Visit(conversionExpression.Operand);
                var type = _typeResolver.ResolveType(conversionExpression.Type);
                return Expression.Convert(exp, type);
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

            private Expression VisitCollection(RLinq.CollectionExpression collectionExpression)
            {
                var collection =
                    from exp in collectionExpression.List
                    select exp.Value;

                var elementType = _typeResolver.ResolveType(collectionExpression.ElementType);
                if (elementType == typeof(object))
                {
                    return Expression.Constant(collection.ToArray());
                }
                else
                {
                    var objectCollectionExpression = Expression.Constant(collection.ToArray());
                    var typeConvertionMethodExpression = Expression.Call(null, MethodInfos.Enumerable.OfType.MakeGenericMethod(elementType), objectCollectionExpression);
                    var toArrayMethodExpression = Expression.Call(null, MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType), typeConvertionMethodExpression);
                    var obj = Expression.Lambda(toArrayMethodExpression).Compile().DynamicInvoke();
                    return Expression.Constant(obj);
                }
            }

            private Expression VisitBinary(RLinq.BinaryExpression binaryExpression)
            {
                var p1 = Visit(binaryExpression.LeftOperand);
                var p2 = Visit(binaryExpression.RightOperand);
                var conversion = Visit(binaryExpression.Conversion) as LambdaExpression;

                if (ReferenceEquals(null, conversion))
                {
                    switch (binaryExpression.Operator)
                    {
                        case RLinq.BinaryOperator.StartsWith:
                            return Expression.Call(p1, MethodInfos.String.StartsWith, p2);
                        case RLinq.BinaryOperator.EndsWith:
                            return Expression.Call(p1, MethodInfos.String.EndsWith, p2);
                        case RLinq.BinaryOperator.Contains:
                            return Expression.Call(p1, MethodInfos.String.Contains, p2);
                        case RLinq.BinaryOperator.In:
                            if (p1.Type == typeof(object))
                            {
                                return Expression.Call(null, MethodInfos.Enumerable.Contains.MakeGenericMethod(p1.Type), p2, p1);
                            }
                            else
                            {
                                var typeConvertionMethod = Expression.Call(null, MethodInfos.Enumerable.OfType.MakeGenericMethod(p1.Type), p2);
                                var toArrayMethodExpression = Expression.Call(null, MethodInfos.Enumerable.ToArray.MakeGenericMethod(p1.Type), typeConvertionMethod);
                                var obj = Expression.Lambda(toArrayMethodExpression).Compile().DynamicInvoke();
                                var exp = Expression.Constant(obj);
                                return Expression.Call(null, MethodInfos.Enumerable.Contains.MakeGenericMethod(p1.Type), exp, p1);
                            }
                        //default:{
                        //    var type = TranslateBinaryOperator(binaryExpression.Operator);
                        //    return Expression.MakeBinary(type, p1, p2);
                    }
                }

                var type = TranslateBinaryOperator(binaryExpression.Operator);
                if (!ReferenceEquals(null, binaryExpression.Method))
                {
                    var method = binaryExpression.Method.ResolveMethod(_typeResolver);
                    if (!ReferenceEquals(null, conversion))
                    {
                        return Expression.MakeBinary(type, p1, p2, binaryExpression.IsLiftedToNull, method, conversion);
                    }
                    else
                    {
                        return Expression.MakeBinary(type, p1, p2, binaryExpression.IsLiftedToNull, method);
                    }
                }
                else
                {
                    return Expression.MakeBinary(type, p1, p2);
                }
            }

            private Expression VisitLambda(RLinq.LambdaExpression lambdaExpression)
            {
                var body = Visit(lambdaExpression.Expression);
                var parameters =
                    from p in lambdaExpression.Parameters
                    select VisitParameter(p);
                return Expression.Lambda(body, parameters.ToArray());
            }

            private static ExpressionType TranslateBinaryOperator(RLinq.BinaryOperator @operator)
            {
                switch (@operator)
                {
                    case RLinq.BinaryOperator.Add:
                        return ExpressionType.Add;
                    case RLinq.BinaryOperator.And:
                        return ExpressionType.AndAlso;
                    case RLinq.BinaryOperator.BitwiseAnd:
                        return ExpressionType.And;
                    case RLinq.BinaryOperator.BitwiseOr:
                        return ExpressionType.Or;
                    case RLinq.BinaryOperator.Coalesce:
                        return ExpressionType.Coalesce;
                    case RLinq.BinaryOperator.Contains:
                        throw new Exception("needs translation into method call expression");
                    case RLinq.BinaryOperator.Divide:
                        return ExpressionType.Divide;
                    case RLinq.BinaryOperator.EndsWith:
                        throw new Exception("needs translation into method call expression");
                    case RLinq.BinaryOperator.Equal:
                        return ExpressionType.Equal;
                    case RLinq.BinaryOperator.ExclusiveOr:
                        return ExpressionType.ExclusiveOr;
                    case RLinq.BinaryOperator.GreaterThan:
                        return ExpressionType.GreaterThan;
                    case RLinq.BinaryOperator.GreaterThanOrEqual:
                        return ExpressionType.GreaterThanOrEqual;
                    case RLinq.BinaryOperator.In:
                        throw new Exception("needs translation into method call expression");
                    case RLinq.BinaryOperator.LessThan:
                        return ExpressionType.LessThan;
                    case RLinq.BinaryOperator.LessThanOrEqual:
                        return ExpressionType.LessThanOrEqual;
                    //case Filter.BinaryOperator.Like:
                    case RLinq.BinaryOperator.Modulo:
                        return ExpressionType.Modulo;
                    case RLinq.BinaryOperator.Multiply:
                        return ExpressionType.Multiply;
                    case RLinq.BinaryOperator.NotEqual:
                        return ExpressionType.NotEqual;
                    //case Filter.BinaryOperator.NotLike:
                    case RLinq.BinaryOperator.Or:
                        return ExpressionType.OrElse;
                    case RLinq.BinaryOperator.StartsWith:
                        throw new Exception("needs translation into method call expression");
                    case RLinq.BinaryOperator.Subtract:
                        return ExpressionType.Subtract;
                    default:
                        throw new Exception(string.Format("Unknown binary operation: '{0}'", @operator));
                }
            }
        }
    }
}