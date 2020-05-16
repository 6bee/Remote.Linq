// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryService : IQueryService
    {
        private static readonly Func<Type, IQueryable> _queryableResourceProvider = type =>
        {
            InMemoryDataStore dataStore = InMemoryDataStore.Instance;

            if (type == typeof(Common.Model.ProductCategory))
            {
                return
                    from x in dataStore.ProductCategories
                    select new Common.Model.ProductCategory
                    {
                        Id = x.Id,
                        Name = x.Name,
                    };
            }

            if (type == typeof(Common.Model.Product))
            {
                return
                    from x in dataStore.Products
                    select new Common.Model.Product
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        ProductCategoryId = x.ProductCategoryId,
                    };
            }

            if (type == typeof(Common.Model.OrderItem))
            {
                return
                    from x in dataStore.OrderItems
                    select new Common.Model.OrderItem
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                    };
            }

            throw new Exception(string.Format("No queryable resource available for type {0}", type));
        };

        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            return queryExpression.Execute(_queryableResourceProvider);
        }
    }
}
