// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;
    using System.Linq;

    public class QueryableResourceDescriptorVisitor : RemoteExpressionVisitorBase
    {
        internal protected QueryableResourceDescriptorVisitor(ITypeResolver typeResolver)
            : base(typeResolver)
        {
        }

        internal Expression ReplaceQueryablesByResourceDescriptors(Expression expression)
        {
            return Visit(expression);
        }

        protected override ConstantExpression VisitConstant(ConstantExpression expression)
        {
            if (expression.Value is IQueryable)
            {
                var queryable = ((IQueryable)expression.Value);
                var elementType = queryable.ElementType;
                var queryableResourceDescriptor = new QueryableResourceDescriptor(elementType);

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
