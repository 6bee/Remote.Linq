// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BindingFlags = System.Reflection.BindingFlags;
    using MemberTypes = Aqua.TypeSystem.MemberTypes;
    using TypeHelper = Aqua.TypeSystem.TypeHelper;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SystemExpressionReWriter
    {
        internal static Expression ReplaceAnonymousTypes(this Expression expression)
            => new AnonymousTypesReplacer().ReWrite(expression);

        public static Expression ResolveDynamicPropertySelectors(this Expression expression, bool throwOnInvalidProperty = false)
            => new DynamicPropertyResolver(throwOnInvalidProperty).Resolve(expression);

        public static LambdaExpression ResolveDynamicPropertySelectors(this LambdaExpression expression, bool throwOnInvalidProperty = false)
            => (LambdaExpression)ResolveDynamicPropertySelectors((Expression)expression, throwOnInvalidProperty);

        /// <summary>
        /// Replace complicated access to <see cref="IRemoteQueryable"/> by simple <see cref="ConstantExpression"/>.
        /// </summary>
        public static Expression SimplifyIncorporationOfRemoteQueryables(this Expression expression)
            => new RemoteQueryableVisitor().Simplify(expression);

        private sealed class AnonymousTypesReplacer : ExpressionVisitorBase
        {
            private static readonly ConstructorInfo _dynamicPropertyContructorInfo = typeof(Property).GetConstructor(new[] { typeof(string), typeof(object) });
            private static readonly ConstructorInfo _dynamicObjectContructorInfo = typeof(DynamicObject).GetConstructor(new[] { typeof(IEnumerable<Property>) });
            private static readonly MethodInfo _dynamicObjectGetMethod = typeof(DynamicObject).GetMethod(nameof(DynamicObject.Get));

            private readonly Dictionary<ParameterExpression, ParameterExpression> _parameterMap = new Dictionary<ParameterExpression, ParameterExpression>();

            public Expression ReWrite(Expression expression)
            {
                Expression projection;
                if (IsProjection(expression))
                {
                    projection = expression;
                }
                else
                {
                    var elementType = TypeHelper.GetElementType(expression.Type);
                    var projectionMethod = MethodInfos.Queryable.Select.MakeGenericMethod(elementType, elementType);
                    var parameter = Expression.Parameter(elementType, "x");
                    var lambda = Expression.Lambda(parameter, parameter);
                    projection = Expression.Call(null, projectionMethod, expression, lambda);
                }

                return Visit(projection);
            }

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                if (node.Member.DeclaringType.IsAnonymousType() ||
                    (node.Member.DeclaringType.IsGenericType && node.Member.DeclaringType.GetGenericArguments().Any(x => x.IsAnonymousType())))
                {
                    var name = node.Member.Name;
                    var instance = Visit(node.Expression);
                    if (instance.Type == typeof(DynamicObject))
                    {
                        var method = _dynamicObjectGetMethod;
                        var memberAccessCallExpression = Expression.Call(instance, method, Expression.Constant(name));
                        var type = ReplaceAnonymousType(node.Type);
                        if (type == typeof(object))
                        {
                            return memberAccessCallExpression;
                        }

                        var conversionExpression = Expression.Convert(memberAccessCallExpression, type);
                        return conversionExpression;
                    }

                    var member = instance.Type.GetMember(name, node.Member.MemberType, (BindingFlags)0xfffffff);
                    if (member.Length != 1)
                    {
                        throw new RemoteLinqException($"Failed to bind {node.Member.MemberType.ToString().ToLower(CultureInfo.CurrentCulture)} {name} of type {instance.Type}");
                    }

                    return Expression.MakeMemberAccess(instance, member.Single());
                }

                return base.VisitMemberAccess(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (!_parameterMap.TryGetValue(node, out var parameter))
                {
                    var type = ReplaceAnonymousType(node.Type);
                    if (ReferenceEquals(type, node.Type))
                    {
                        parameter = node;
                    }
                    else
                    {
                        parameter = Expression.Parameter(type, node.Name);
                    }

                    _parameterMap[node] = parameter;
                }

                return parameter;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                var instance = Visit(node.Object);
                var arguments = VisitExpressionList(node.Arguments);
                if (!ReferenceEquals(instance, node.Object) || !ReferenceEquals(arguments, node.Arguments))
                {
                    var method = ReplaceAnonymousType(node.Method);
                    return Expression.Call(instance, method, arguments);
                }

                return node;
            }

            protected override NewExpression VisitNew(NewExpression node)
            {
                if (node.Type.IsAnonymousType())
                {
                    if (node.Members?.Any() != true)
                    {
                        throw new RemoteLinqException("Missing members definition for anonymous generic type.");
                    }

                    var args = VisitExpressionList(node.Arguments);
                    var arguments = args.Select((a, i) =>
                     {
                         var arg = a.Type == typeof(object) ? a : Expression.Convert(a, typeof(object));
                         return (Expression)Expression.New(
                             _dynamicPropertyContructorInfo,
                             Expression.Constant(GetMemberName(node.Members[i])),
                             arg);
                     });

                    var argsExpression = Expression.NewArrayInit(_dynamicPropertyContructorInfo.DeclaringType, arguments);
                    return Expression.New(_dynamicObjectContructorInfo, argsExpression);
                }

                return base.VisitNew(node);
            }

            private static string GetMemberName(MemberInfo memberInfo)
            {
                var name = memberInfo.Name;
                if (memberInfo.GetMemberType() == MemberTypes.Method && name.Length > 4 && name.StartsWith("get_", StringComparison.Ordinal))
                {
                    name = name.Substring(4);
                }

                return name;
            }

            protected override Expression VisitLambda(LambdaExpression node)
            {
                var body = Visit(node.Body);
                var type = ReplaceAnonymousType(node.Type);
                var parameters =
                    from p in node.Parameters
                    select Expression.Parameter(ReplaceAnonymousType(p.Type), p.Name);
                return Expression.Lambda(type, body, parameters);
            }

            private static MethodInfo ReplaceAnonymousType(MethodInfo methodInfo)
            {
                if (methodInfo.IsGenericMethod)
                {
                    var args =
                        from a in methodInfo.GetGenericArguments()
                        select ReplaceAnonymousType(a);
                    return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(args.ToArray());
                }

                return methodInfo;
            }

            private static Type ReplaceAnonymousType(Type type)
            {
                if (type.IsGenericType)
                {
                    if (type.IsAnonymousType())
                    {
                        return typeof(DynamicObject);
                    }

                    var args =
                        from a in type.GetGenericArguments()
                        select ReplaceAnonymousType(a);
                    return type.GetGenericTypeDefinition().MakeGenericType(args.ToArray());
                }

                return type;
            }

            private static bool IsProjection(Expression expression)
            {
                if (expression is MethodCallExpression call)
                {
                    var method = call.Method;
                    if (method.DeclaringType == typeof(Queryable) && string.Equals(method.Name, nameof(Queryable.Select), StringComparison.Ordinal))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private sealed class DynamicPropertyResolver : ExpressionVisitorBase
        {
            private readonly bool _throwOnInvalidProperty;

            internal DynamicPropertyResolver(bool throwOnInvalidProperty)
            {
                _throwOnInvalidProperty = throwOnInvalidProperty;
            }

            internal Expression Resolve(Expression expression) => Visit(expression);

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (string.Equals(node.Method.Name, "get_Item", StringComparison.Ordinal) && node.Arguments.Count == 1)
                {
                    var expression = node.Arguments[0];
                    if (expression.Type == typeof(string))
                    {
                        string propertyName;
                        if (expression.NodeType == ExpressionType.Constant)
                        {
                            var exp = (ConstantExpression)expression;
                            propertyName = (string)exp.Value;
                        }
                        else
                        {
                            var lambda = Expression.Lambda<Func<object>>(expression);
                            var value = lambda.Compile()();
                            propertyName = (string)value;
                        }

                        if (propertyName is not null)
                        {
                            var instance = Visit(node.Object);
                            var propertyInfo = node.Object.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (propertyInfo is not null)
                            {
                                return Expression.MakeMemberAccess(instance, propertyInfo);
                            }

                            if (_throwOnInvalidProperty)
                            {
                                throw new RemoteLinqException($"'{propertyName}' is either not a valid or an ambiguous property of type {node.Object.Type.FullName}");
                            }
                        }
                    }
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitLambda(LambdaExpression node)
            {
                var body = Visit(node.Body);
                if (body != node.Body)
                {
                    var type = node.Type;
                    if (type.IsGenericType)
                    {
                        var genericTypeDefinition = type.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Func<,>))
                        {
                            var genericArguments = type.GetGenericArguments().ToArray();
                            if (genericArguments[1] != body.Type)
                            {
                                type = typeof(Func<,>).MakeGenericType(genericArguments[0], body.Type);
                            }
                        }
                    }

                    return Expression.Lambda(type, body, node.Parameters);
                }

                return node;
            }
        }

        [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Owned disposable type serves special purpose")]
        private sealed class RemoteQueryableVisitor : ExpressionVisitorBase
        {
            private sealed class ParameterScope : IDisposable
            {
                private readonly ParameterScope? _parent;
                private readonly RemoteQueryableVisitor _visitor;
                private int _count;

                internal ParameterScope(RemoteQueryableVisitor visitor)
                    : this(visitor, null)
                {
                }

                private ParameterScope(RemoteQueryableVisitor visitor, ParameterScope? parent)
                {
                    _parent = parent;
                    _count = parent?._count ?? 0;

                    _visitor = visitor.CheckNotNull(nameof(visitor));
                    _visitor._parameterScope = this;
                }

                internal void Increment() => _count++;

                void IDisposable.Dispose() => _visitor._parameterScope = _parent!;

                internal bool HasParameters => _count > 0;

                internal IDisposable NewScope() => new ParameterScope(_visitor, this);
            }

            private ParameterScope _parameterScope;

            public RemoteQueryableVisitor()
            {
                _parameterScope = new ParameterScope(this);
            }

            public Expression Simplify(Expression expression) => Visit(expression);

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                using (_parameterScope.NewScope())
                {
                    node = (MemberExpression)base.VisitMemberAccess(node);

                    if (!_parameterScope.HasParameters && typeof(IQueryable).IsAssignableFrom(node.Type))
                    {
                        var lambda = Expression.Lambda<Func<object>>(node);
                        var value = lambda.Compile()();
                        if (value is IRemoteResource)
                        {
                            return Expression.Constant(value, node.Type);
                        }
                    }

                    return node;
                }
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                _parameterScope.Increment();
                return base.VisitParameter(node);
            }
        }
    }
}
