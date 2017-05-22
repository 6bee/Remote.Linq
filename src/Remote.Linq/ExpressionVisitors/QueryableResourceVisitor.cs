// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Reflection;
    using MemberTypes = Aqua.TypeSystem.MemberTypes;
    using FieldInfo = Aqua.TypeSystem.FieldInfo;
    using PropertyInfo = Aqua.TypeSystem.PropertyInfo;
    using MethodInfo = Aqua.TypeSystem.MethodInfo;
    using Aqua.Dynamic;

    public class QueryableResourceVisitor
    {
        internal T ReplaceResourceDescriptorsByQueryable<T>(T expression, Func<Type, IQueryable> provider, ITypeResolver typeResolver) where T : Expression
        {
            return (T)new ResourceDescriptorVisitor(provider, typeResolver).ReplaceResourceDescriptorsByQueryable(expression);
        }

        internal Expression ReplaceQueryablesByResourceDescriptors(Expression expression, ITypeResolver typeResolver)
        {
            return new QueryableVisitor(typeResolver).ReplaceQueryablesByResourceDescriptors(expression);
        }

        protected class ResourceDescriptorVisitor : RemoteExpressionVisitorBase
        {
            protected readonly ITypeResolver _typeResolver;
            private readonly Func<Type, IQueryable> _provider;

            internal protected ResourceDescriptorVisitor(Func<Type, IQueryable> provider, ITypeResolver typeResolver)
            {
                _provider = provider;
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            internal Expression ReplaceResourceDescriptorsByQueryable(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var type = expression.Type.ResolveType(_typeResolver);
                if (type == typeof(QueryableResourceDescriptor) && !ReferenceEquals(null, expression.Value))
                {
                    var queryableResourceDescriptor = (QueryableResourceDescriptor)expression.Value;
                    var queryableType = queryableResourceDescriptor.Type.ResolveType(_typeResolver);
                    var queryable = _provider(queryableType);
                    return new ConstantExpression(queryable);
                }

                if (type == typeof(ConstantQueryArgument) && !ReferenceEquals(null, expression.Value))
                {
                    var newConstantQueryArgument = new ConstantQueryArgument((ConstantQueryArgument)expression.Value);
                    var properties = newConstantQueryArgument.Properties ?? Enumerable.Empty<Property>();
                    foreach (var property in properties)
                    {
                        var queryableResourceDescriptor = property.Value as QueryableResourceDescriptor;
                        if (!ReferenceEquals(null, queryableResourceDescriptor))
                        {
                            var queryableType = queryableResourceDescriptor.Type.ResolveType(_typeResolver);
                            var queryable = _provider(queryableType);
                            property.Value = queryable;
                        }
                    }

                    return new ConstantExpression(newConstantQueryArgument);
                }

                return base.VisitConstant(expression);
            }
        }

        protected class QueryableVisitor : RemoteExpressionVisitorBase
        {
            protected readonly ITypeResolver _typeResolver;

            internal protected QueryableVisitor(ITypeResolver typeResolver)
            {
                _typeResolver = typeResolver ?? TypeResolver.Instance;
            }

            internal Expression ReplaceQueryablesByResourceDescriptors(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var queryable = AsQueryableOrNull(expression.Value);
                if (!ReferenceEquals(null, queryable))
                {
                    var queryableResourceDescriptor = new QueryableResourceDescriptor(queryable.ElementType);
                    return new ConstantExpression(queryableResourceDescriptor);
                }

                var constantQueryArgument = expression.Value as ConstantQueryArgument;
                if (!ReferenceEquals(null, constantQueryArgument))
                {
                    var newConstantQueryArgument = new ConstantQueryArgument(constantQueryArgument);
                    var properties = newConstantQueryArgument.Properties ?? Enumerable.Empty<Property>();
                    foreach (var property in properties)
                    {
                        var value = AsQueryableOrNull(property.Value);
                        if (!ReferenceEquals(null, value))
                        {
                            var queryableResourceDescriptor = new QueryableResourceDescriptor(value.ElementType);
                            property.Value = queryableResourceDescriptor;
                        }
                    }

                    return new ConstantExpression(newConstantQueryArgument, expression.Type);
                }

                return base.VisitConstant(expression);
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                var member = expression.Member;
                Type type;
                Func<object, object> access;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        var field = ((FieldInfo)member).ResolveField(_typeResolver);
                        type = field.FieldType;
                        access = field.GetValue;
                        break;

                    case MemberTypes.Property:
                        var property = ((PropertyInfo)member).ResolveProperty(_typeResolver);
                        type = property.PropertyType;
                        access = property.GetValue;
                        break;

                    case MemberTypes.Method:
                        var method = ((MethodInfo)member).ResolveMethod(_typeResolver);
                        type = method.ReturnType;
                        access = x => method.Invoke(x, null);
                        break;

                    default:
                        type = null;
                        access = null;
                        break;
                }

                if (!ReferenceEquals(null, type) && type.IsGenericType() && typeof(IQueryable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    if (ReferenceEquals(null, expression.Expression))
                    {
                        var elementType = type.GetGenericArguments().Single();
                        var queryableResourceDescriptor = new QueryableResourceDescriptor(elementType);
                        return new ConstantExpression(queryableResourceDescriptor);
                    }
                    else
                    {
                        // NOTE: Examine actual value to make sure it's an `IQueryable` 
                        //       to be treated as queryable resource rather than an `EnumerableQuery<>`.
                        //       For `EnumerableQuery<>` we ship the enumeration as part of the expression.
                        var exp = expression.Expression.ToLinqExpression();
                        var lambda = System.Linq.Expressions.Expression.Lambda<Func<object>>(exp);
                        var instance = lambda.Compile()();
                        var value = access(instance);
                        var queryable = AsQueryableOrNull(value);
                        if (!ReferenceEquals(null, queryable))
                        {
                            var queryableResourceDescriptor = new QueryableResourceDescriptor(queryable.ElementType);
                            return new ConstantExpression(queryableResourceDescriptor);
                        }
                    }
                }

                return base.VisitMemberAccess(expression);
            }

            private static IQueryable AsQueryableOrNull(object value)
            {
                var queryable = value as IQueryable;
                if (ReferenceEquals(null, queryable))
                {
                    return null;
                }

                var type = queryable.GetType();
                if (type.IsGenericType() && typeof(EnumerableQuery<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    return null;
                }

                return queryable;
            }
        }
    }
}
