// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;

    public class QueryService : IQueryService
    {
        private class QueryExecutor : ExpressionExecutor
        {
            private QueryExecutor()
                : base(QueryableResourceProvider)
            {
            }

            public static QueryExecutor Instance { get; } = new QueryExecutor();

            public new object Execute(Expression expression)
            {
                Expression preparedRemoteExpression = Prepare(expression);
                System.Linq.Expressions.Expression linqExpression = Transform(preparedRemoteExpression);
                System.Linq.Expressions.Expression preparedLinqExpression = Prepare(linqExpression);
                object queryResult = Execute(preparedLinqExpression);
                return queryResult;
            }

            private static IQueryable QueryableResourceProvider(Type type)
            {
                InMemoryDataStore dataStore = InMemoryDataStore.Instance;

                if (type == typeof(ProductCategory))
                {
                    return dataStore.ProductCategories.AsQueryable();
                }

                if (type == typeof(Product))
                {
                    return dataStore.Products.AsQueryable();
                }

                if (type == typeof(OrderItem))
                {
                    return dataStore.OrderItems.AsQueryable();
                }

                throw new Exception($"No queryable resource available for type {type}");
            }
        }

        public object ExecuteQuery(Expression queryExpression)
            => QueryExecutor.Instance.Execute(queryExpression);
    }
}
