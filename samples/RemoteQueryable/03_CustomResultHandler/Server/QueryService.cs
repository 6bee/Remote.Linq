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

            throw new NotSupportedException($"No queryable resource available for type {type}");
        }

        public object ExecuteQuery(Expression queryExpression) => queryExpression.Execute<object>(QueryableResourceProvider);
    }
}
