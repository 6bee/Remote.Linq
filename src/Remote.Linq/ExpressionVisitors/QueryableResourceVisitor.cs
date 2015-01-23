// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionVisitors
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.TypeSystem;
    using System;

    public class QueryableResourceVisitor : RemoteExpressionVisitorBase
    {
        private readonly Func<Type, System.Linq.IQueryable> _provider;

        internal protected QueryableResourceVisitor(Func<Type, System.Linq.IQueryable> provider, ITypeResolver typeResolver)
            : base(typeResolver)
        {
            _provider = provider;
        }

        internal Expression ReplaceResourceDescriptorsByQueryable(Expression expression)
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
}
