// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;

    public class CustomExpressionExecutor : ExpressionExecutor
    {
        public CustomExpressionExecutor()
            : base(QueryableResourceProvider)
        {
        }

        public new object Execute(Expression expression)
        {
            Remote.Linq.Expressions.Expression preparedRemoteExpression = Prepare(expression);
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
}
