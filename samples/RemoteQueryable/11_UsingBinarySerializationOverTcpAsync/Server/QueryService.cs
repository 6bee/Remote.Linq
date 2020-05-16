// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class QueryService
    {
        private static readonly Func<Type, IQueryable> _queryableResourceProvider = type =>
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
        };

        public Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression)
        {
            // Note: there is no async version of IQueryable yet, but there is e.g. for EF Core.
            // Async methods awailable with `Remote.Linq.EntityFramework` and `Remote.Linq.EntityFrameworkCore` version 6 and later.
            return Task.Run(() => queryExpression.Execute(_queryableResourceProvider));
        }
    }
}
