﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryService : IQueryService
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

        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            var mapper = new DynamicObjectMapper(isKnownTypeProvider: new IsKnownTypeProvider());
            return queryExpression.Execute(_queryableResourceProvider, mapper: mapper);
        }
    }
}