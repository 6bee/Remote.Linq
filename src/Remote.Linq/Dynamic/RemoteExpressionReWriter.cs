// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Expressions;
using Remote.Linq.TypeSystem;
using System;
using System.Linq;

namespace Remote.Linq.Dynamic
{
    internal static class RemoteExpressionReWriter
    {
        internal static Expression ReplaceResourceDescriptorsByQueryable(this Expression expression, Func<Type, System.Linq.IQueryable> provider = null)
        {
            return new QueryableResourceVisitor(provider ?? ProviderRegistry.QueryableResourceProvider).ReplaceResourceDescriptorsByQueryable(expression);
        }

        internal static Expression ReplaceQueryableByResourceDescriptors(this Expression expression)
        {
            return new QueryableResourceDescriptorVisitor().ReplaceQueryableResourceDescriptors(expression);
        }

        private sealed class QueryableResourceVisitor : RemoteExpressionVisitorBase
        {
            private readonly Func<Type, System.Linq.IQueryable> _provider;

            public QueryableResourceVisitor(Func<Type, System.Linq.IQueryable> provider)
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
                if (expression.Type.Type == typeof(QueryableResourceDescriptor) && !ReferenceEquals(null, expression.Value))
                {
                    var queryableResourceDescriptor = (QueryableResourceDescriptor)expression.Value;
                    var queryable = _provider(queryableResourceDescriptor.Type);
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
