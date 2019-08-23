// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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
            => new DynamicPropertyResolver(throwOnInvalidProperty).ResolveDynamicPropertySelectors(expression);

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
            private static readonly MethodInfo _dynamicObjectGetMethod = typeof(DynamicObject).GetMethod("Get");

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

            protected override Expression VisitMemberAccess(MemberExpression m)
            {
                if (m.Member.DeclaringType.IsAnonymousType() || (m.Member.DeclaringType.IsGenericType() && m.Member.DeclaringType.GetGenericArguments().Any(x => x.IsAnonymousType())))
                {
                    var name = m.Member.Name;
                    var instance = Visit(m.Expression);
                    if (instance.Type == typeof(DynamicObject))
                    {
                        var method = _dynamicObjectGetMethod;
                        var memberAccessCallExpression = Expression.Call(instance, method, Expression.Constant(name));
                        var type = ReplaceAnonymousType(m.Type);
                        if (type == typeof(object))
                        {
                            return memberAccessCallExpression;
                        }
                        else
                        {
                            var conversionExpression = Expression.Convert(memberAccessCallExpression, type);
                            return conversionExpression;
                        }
                    }
                    else
                    {
                        var member = instance.Type.GetMember(name, m.Member.GetMemberType(), (BindingFlags)0xfffffff);
                        if (member.Count() != 1)
                        {
                            throw new Exception($"Failed to bind {m.Member.GetMemberType().ToString().ToLower()} {name} of type {instance.Type}");
                        }

                        return Expression.MakeMemberAccess(instance, member.Single());
                    }
                }
                else
                {
                    return base.VisitMemberAccess(m);
                }
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression parameter;
                if (!_parameterMap.TryGetValue(p, out parameter))
                {
                    var type = ReplaceAnonymousType(p.Type);
                    if (ReferenceEquals(type, p.Type))
                    {
                        parameter = p;
                    }
                    else
                    {
                        parameter = Expression.Parameter(type, p.Name);
                    }

                    _parameterMap[p] = parameter;
                }

                return parameter;
            }

            protected override Expression VisitMethodCall(MethodCallExpression call)
            {
                var instance = Visit(call.Object);
                var arguments = VisitExpressionList(call.Arguments);
                if (!ReferenceEquals(instance, call.Object) || !ReferenceEquals(arguments, call.Arguments))
                {
                    var method = ReplaceAnonymousType(call.Method);
                    return Expression.Call(instance, method, arguments);
                }
                else
                {
                    return call;
                }
            }

            protected override NewExpression VisitNew(NewExpression newExpression)
            {
                if (newExpression.Type.IsAnonymousType())
                {
                    IEnumerable<Expression> arguments;
                    if (newExpression.Members != null && newExpression.Members.Any())
                    {
                        var args = VisitExpressionList(newExpression.Arguments);
                        arguments = args.Select((a, i) =>
                        {
                            var arg = a.Type == typeof(object) ? a : Expression.Convert(a, typeof(object));
                            return (Expression)Expression.New(
                                _dynamicPropertyContructorInfo,
                                Expression.Constant(GetMemberName(newExpression.Members[i])),
                                arg);
                        });
                    }
                    else
                    {
                        throw new NotImplementedException("unecpected case");
                    }

                    var argsExpression = Expression.NewArrayInit(_dynamicPropertyContructorInfo.DeclaringType, arguments);
                    var x = Expression.New(_dynamicObjectContructorInfo, argsExpression);
                    return x;
                }
                else
                {
                    return base.VisitNew(newExpression);
                }
            }

            private static string GetMemberName(MemberInfo memberInfo)
            {
                var name = memberInfo.Name;
                if (memberInfo.GetMemberType() == MemberTypes.Method && name.Length > 4 && name.StartsWith("get_"))
                {
                    name = name.Substring(4, name.Length - 4);
                }

                return name;
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                var body = Visit(lambda.Body);
                var type = ReplaceAnonymousType(lambda.Type);
                var parameters =
                    from p in lambda.Parameters
                    select Expression.Parameter(ReplaceAnonymousType(p.Type), p.Name);
                return Expression.Lambda(type, body, parameters);
            }

            private MethodInfo ReplaceAnonymousType(MethodInfo methodInfo)
            {
                if (methodInfo.IsGenericMethod)
                {
                    var args =
                        from a in methodInfo.GetGenericArguments()
                        select ReplaceAnonymousType(a);
                    return methodInfo.GetGenericMethodDefinition().MakeGenericMethod(args.ToArray());
                }
                else
                {
                    return methodInfo;
                }
            }

            private Type ReplaceAnonymousType(Type type)
            {
                if (type.IsGenericType())
                {
                    if (type.IsAnonymousType())
                    {
                        return typeof(DynamicObject);
                    }
                    else
                    {
                        var args =
                            from a in type.GetGenericArguments()
                            select ReplaceAnonymousType(a);
                        return type.GetGenericTypeDefinition().MakeGenericType(args.ToArray());
                    }
                }
                else
                {
                    return type;
                }
            }

            private static bool IsProjection(Expression expression)
            {
                var call = expression as MethodCallExpression;
                if (call != null)
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

            internal Expression ResolveDynamicPropertySelectors(Expression expression)
            {
                return Visit(expression);
            }

            protected override Expression VisitMethodCall(MethodCallExpression m)
            {
                if (m.Method.Name == "get_Item" && m.Arguments.Count == 1)
                {
                    string propertyName = null;

                    var expression = m.Arguments[0];
                    if (expression.Type == typeof(string))
                    {
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

                        if (!(propertyName is null))
                        {
                            var instance = Visit(m.Object);
                            var propertyInfo = m.Object.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                            if (!(propertyInfo is null))
                            {
                                return Expression.MakeMemberAccess(instance, propertyInfo);
                            }

                            if (_throwOnInvalidProperty)
                            {
                                throw new Exception($"'{propertyName}' is not a valid or an ambiguous property of type {m.Object.Type.FullName}");
                            }
                        }
                    }
                }

                return base.VisitMethodCall(m);
            }

            protected override Expression VisitLambda(LambdaExpression lambda)
            {
                Expression body = Visit(lambda.Body);
                if (body != lambda.Body)
                {
                    var type = lambda.Type;
                    if (type.IsGenericType())
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

                    return Expression.Lambda(type, body, lambda.Parameters);
                }

                return lambda;
            }
        }

        private sealed class RemoteQueryableVisitor : ExpressionVisitorBase
        {
            private class ParameterScope : IDisposable
            {
                private readonly ParameterScope _parent;
                private readonly RemoteQueryableVisitor _visitor;
                private int _count;

                internal ParameterScope(RemoteQueryableVisitor visitor)
                    : this(visitor, null)
                {
                }

                private ParameterScope(RemoteQueryableVisitor visitor, ParameterScope parent)
                {
                    _parent = parent;
                    _count = parent?._count ?? 0;

                    _visitor = visitor;
                    _visitor._parameterScope = this;
                }

                internal void Increment() => _count++;

                void IDisposable.Dispose() => _visitor._parameterScope = _parent;

                internal bool HasParameters => _count > 0;

                internal IDisposable NewScope() => new ParameterScope(_visitor, this);
            }

            private ParameterScope _parameterScope;

            public RemoteQueryableVisitor()
            {
                _parameterScope = new ParameterScope(this);
            }

            public Expression Simplify(Expression expression)
                => Visit(expression);

            protected override Expression VisitMemberAccess(MemberExpression node)
            {
                using (_parameterScope.NewScope())
                {
                    node = (MemberExpression)base.VisitMemberAccess(node);

                    if (!_parameterScope.HasParameters && typeof(IQueryable).IsAssignableFrom(node.Type))
                    {
                        var lambda = Expression.Lambda<Func<object>>(node);
                        var value = lambda.Compile()();
                        if (value is IRemoteQueryable)
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
