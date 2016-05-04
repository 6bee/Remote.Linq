// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryService
    {
        private static readonly IDynamicObjectMapper _dynamicObjectMapper;
        private static readonly Func<Type, IQueryable> _queryableResourceProvider;

        static QueryService()
        {
            _dynamicObjectMapper = new DynamicObjectMapper(settings: new DynamicObjectMapperSettings { FormatPrimitiveTypesAsString = true });

            _queryableResourceProvider = type =>
            {
                var dataStore = InMemoryDataStore.Instance;

                if (type == typeof(ProductCategory)) return dataStore.ProductCategories.AsQueryable();
                if (type == typeof(Product)) return dataStore.Products.AsQueryable();
                if (type == typeof(OrderItem)) return dataStore.OrderItems.AsQueryable();

                throw new Exception(string.Format("No queryable resource available for type {0}", type));
            };
        }

        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            return queryExpression.Execute(_queryableResourceProvider, mapper: _dynamicObjectMapper);
        }
    }
}
