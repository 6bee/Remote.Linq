// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common.Model;
using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Server
{
    public class QueryController : ApiController
    {
        private static readonly Func<Type, IQueryable> _queryableResourceProvider = type =>
        {
            var dataStore = InMemoryDataStore.Instance;

            if (type == typeof(ProductCategory)) return dataStore.ProductCategories.AsQueryable();
            if (type == typeof(Product)) return dataStore.Products.AsQueryable();
            if (type == typeof(OrderItem)) return dataStore.OrderItems.AsQueryable();

            throw new Exception(string.Format("No queryable resource available for type {0}", type));
        };

        public IEnumerable<DynamicObject> Query([FromBody] Expression expression)
        {
            try
            {
                return expression.Execute(_queryableResourceProvider);
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("{0}: {1}", ex.GetType(), ex.Message);
                var errorMessageData = Encoding.UTF8.GetBytes(errorMessage);
                
                var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new ByteArrayContent(errorMessageData)
                };

                throw new HttpResponseException(httpResponse);
            }
        }
    }
}
