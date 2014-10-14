// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Expressions;
using Remote.Linq.TypeSystem;
using System;
using System.ComponentModel;
using System.Linq;

namespace Remote.Linq.Dynamic
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteExpressionReWriter
    {
        public static Expression ReplaceResourceDescriptorsByQueryable(this Expression expression, ITypeResolver typeResolver = null, Func<Type, System.Linq.IQueryable> provider = null)
        {
            return new QueryableResourceVisitor(provider ?? ProviderRegistry.QueryableResourceProvider, typeResolver).ReplaceResourceDescriptorsByQueryable(expression);
        }

        public static Expression ReplaceQueryableByResourceDescriptors(this Expression expression, ITypeResolver typeResolver = null)
        {
            return new QueryableResourceDescriptorVisitor(typeResolver).ReplaceQueryableResourceDescriptors(expression);
        }

        private sealed class QueryableResourceVisitor : RemoteExpressionVisitorBase
        {
            private readonly Func<Type, System.Linq.IQueryable> _provider;

            public QueryableResourceVisitor(Func<Type, System.Linq.IQueryable> provider, ITypeResolver typeResolver)
                : base(typeResolver)
            {
                _provider = provider;
            }

            public Expression ReplaceResourceDescriptorsByQueryable(Expression expression)
            {
                var result = Visit(expression);
                return result;
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                var type = _typeResolver.ResolveType(expression.Type);
                if (type == typeof(QueryableResourceDescriptor) && !ReferenceEquals(null, expression.Value))
                {
                    var queryableResourceDescriptor = (QueryableResourceDescriptor)expression.Value;
                    var queryableType = _typeResolver.ResolveType(queryableResourceDescriptor.Type);
                    var queryable = _provider(queryableType);
                    return Expression.Constant(queryable);
                }
                else
                {
                    return base.VisitConstant(expression);
                }
            }
        }

        private sealed class QueryableResourceDescriptorVisitor : RemoteExpressionVisitorBase
        {
            public QueryableResourceDescriptorVisitor(ITypeResolver typeResolver)
                : base(typeResolver)
            {
            }

            public Expression ReplaceQueryableResourceDescriptors(Expression expression)
            {
                return Visit(expression);
            }

            protected override ConstantExpression VisitConstant(ConstantExpression expression)
            {
                if (!ReferenceEquals(null, expression.Value) && typeof(IQueryable).IsAssignableFrom(expression.Value.GetType()))
                {
                    var queryableResourceDescriptor = new QueryableResourceDescriptor(((IQueryable)expression.Value).ElementType);
                    return Expression.Constant(queryableResourceDescriptor);
                }
                else
                {
                    return base.VisitConstant(expression);
                }
            }

            protected override Expression VisitMemberAccess(MemberExpression expression)
            {
                //TODO: use _typeResolver
                var member = expression.Member;
                Type type;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        type = ((FieldInfo)member).Field.FieldType;
                        break;
                    case MemberTypes.Property:
                        type = ((PropertyInfo)member).Property.PropertyType;
                        break;
                    case MemberTypes.Method:
                        type = ((MethodInfo)member).Method.ReturnType;
                        break;
                    default:
                        type = null;
                        break;
                }
                if (!ReferenceEquals(null, type) && type.IsGenericType() && typeof(IQueryable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    var elementType = type.GetGenericArguments().Single();
                    var queryableResourceDescriptor = new QueryableResourceDescriptor(elementType);
                    return Expression.Constant(queryableResourceDescriptor);
                }
                return base.VisitMemberAccess(expression);
            }
        }
    }
}
