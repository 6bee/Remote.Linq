// Copyright (c) Christof Senn. All rights reserved. 

using Common.DataContract;
using Common.ServiceContract;
using Remote.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server
{
    public class RemoteLinqDataService : IRemoteLinqDataService
    {
        public IEnumerable<Product> GetProducts(Query<Product> query)
        {
            var result = DataSource.Products
                .ApplyQuery(query)
                .ToList();
            return result;
        }

        public IEnumerable<Order> GetOrders(Query<Order> query)
        {
            var result = DataSource.Orders
                .ApplyQuery(query)
                .ToList();
            return result;
        }

        public IEnumerable<object> GetData(Query query)
        {
            var result = typeof(RemoteLinqDataService)
                .GetMethod("OpenTypeQuery", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod((Type)query.Type)
                .Invoke(this, new object[] { query });
            return (IEnumerable<object>)result;
        }

        private IEnumerable<T> OpenTypeQuery<T>(IQuery query)
        {
            Query<T> genericQuery = Query<T>.CreateFromNonGeneric(query);
            var data = DataSource.Query<T>()
                .ApplyQuery(genericQuery)
                .ToList();
            return data;
        }
    }
}
