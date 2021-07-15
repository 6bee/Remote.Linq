// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;

    public class QueryService : IQueryService
    {
        private Func<Type, string, IQueryable> QueryableResourceProvider => (type, accessToken) =>
        {
            var dataStore = InMemoryDataStore.Instance;

            if (type == typeof(Common.Model.ProductCategory))
            {
                return
                    from x in dataStore.ProductCategories
                    where accessToken.StartsWith("secure")
                    select new Common.Model.ProductCategory
                    {
                        Id = x.Id,
                        Name = x.Name,
                    };
            }

            if (type == typeof(Common.Model.ProductGroup))
            {
                return
                    from x in dataStore.ProductGroups
                    where accessToken.StartsWith("secure")
                    select new Common.Model.ProductGroup
                    {
                        Id = x.Id,
                        GroupName = x.GroupName,
                        Products = x.Products.ToList(),
                    };
            }

            if (type == typeof(Common.Model.Product))
            {
                return
                    from x in dataStore.Products
                    where accessToken.StartsWith("secure")
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
                    where accessToken.StartsWith("secure")
                    select new Common.Model.OrderItem
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                    };
            }

            throw new NotSupportedException($"No queryable resource available for type {type}");
        };

        public DynamicObject ExecuteQuery(Expression queryExpression, string accessToken)
            => queryExpression.Execute(type => QueryableResourceProvider(type, accessToken));
    }
}
