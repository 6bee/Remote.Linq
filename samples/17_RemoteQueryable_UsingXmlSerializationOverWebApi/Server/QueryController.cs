// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http;

    public class QueryController : ApiController
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

            throw new NotSupportedException($"No queryable resource available for type {type}");
        };

        public IEnumerable<DynamicObject> Query(Expression expression)
        {
            try
            {
                return expression.Execute(_queryableResourceProvider);
            }
            catch (Exception ex)
            {
                string errorMessage = $"{ex.GetType()}: {ex.Message}";
                byte[] errorMessageData = Encoding.UTF8.GetBytes(errorMessage);

                var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ByteArrayContent(errorMessageData),
                };

                throw new HttpResponseException(httpResponse);
            }
        }
    }
}
