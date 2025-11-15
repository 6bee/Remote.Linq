// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common.Model;
using Common.ServiceContract;

public class TraditionalDataService : ITraditionalDataService
{
    private InMemoryDataStore DataSource => InMemoryDataStore.Instance;

    public IEnumerable<Product> GetProductsByName(string productName) =>
        from product in DataSource.Products
        where product.Name == productName
        select product;

    public IEnumerable<Order> GetOrdersByProductId(long productId) =>
        from order in DataSource.Orders
        where order.Items.Any(item => item.ProductId == productId)
        select order;
}