// Copyright (c) Christof Senn. All rights reserved. 

using System.Collections.Generic;
using System.Linq;
using Common.DataContract;
using Common.ServiceContract;

namespace Server
{
    public class DataService : IDataService
    {
        public IEnumerable<Product> GetProductsByName(string productName) =>
            from product in DataSource.Products
            where product.Name == productName
            select product;

        public IEnumerable<Order> GetOrdersByProductId(long productId) =>
            from order in DataSource.Orders
            where order.Items.Any(item => item.ProductId == productId)
            select order;
    }
}
