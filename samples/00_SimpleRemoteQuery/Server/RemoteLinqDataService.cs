// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common.Model;
using Common.ServiceContract;
using Remote.Linq.SimpleQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class RemoteLinqDataService : IRemoteLinqDataService
{
    private InMemoryDataStore DataSource => InMemoryDataStore.Instance;

    private IEnumerable<T> GetQuerySource<T>()
    {
        if (typeof(T) == typeof(Product))
        {
            return (IEnumerable<T>)DataSource.Products;
        }

        if (typeof(T) == typeof(Order))
        {
            return (IEnumerable<T>)DataSource.Orders;
        }

        throw new NotSupportedException($"Data type {typeof(T).FullName} may not be served by this data source.");
    }

    public IEnumerable<Product> GetProducts(Query<Product> query)
        => DataSource.Products
        .ApplyQuery(query)
        .ToList();

    public IEnumerable<Order> GetOrders(Query<Order> query)
        => DataSource.Orders
        .ApplyQuery(query)
        .ToList();

    public IEnumerable<object> GetData(IQuery query)
        => (IEnumerable<object>)typeof(RemoteLinqDataService)
        .GetMethod(nameof(OpenTypeQuery), BindingFlags.Instance | BindingFlags.NonPublic)
        .MakeGenericMethod((Type)query.Type)
        .Invoke(this, new object[] { query });

    private IEnumerable<T> OpenTypeQuery<T>(IQuery query)
    {
        Query<T> genericQuery = query.ToGenericQuery<T>();
        return GetQuerySource<T>()
            .ApplyQuery(genericQuery)
            .ToList();
    }
}