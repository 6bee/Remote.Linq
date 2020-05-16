// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.DataContract;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    internal static class DataSource
    {
        private static long _productCount;
        private static long _orderCount;

        static DataSource()
        {
            List<Product> products = new List<Product>()
            {
                new Product { Id = GetNextProductId(), Name = "Apple", Price = 0.85m },
                new Product { Id = GetNextProductId(), Name = "Pear", Price = 1.2m },
                new Product { Id = GetNextProductId(), Name = "Car", Price = 25999.99m },
                new Product { Id = GetNextProductId(), Name = "House", Price = 680000m },
            };

            List<Order> orders = new List<Order>()
            {
                new Order { Id = GetNextOrderId() },
                new Order { Id = GetNextOrderId() },
                new Order { Id = GetNextOrderId() },
                new Order { Id = GetNextOrderId() },
                new Order { Id = GetNextOrderId() },
            };

            orders[0].AddProduct(products[0], 100);
            orders[0].AddProduct(products[1], 20);
            orders[0].AddProduct(products[2], 1);

            orders[1].AddProduct(products[0], 8000);
            orders[1].AddProduct(products[2], 2);

            orders[2].AddProduct(products[1], 1);

            orders[4].AddProduct(products[1], 1);
            orders[4].AddProduct(products[2], 11);
            orders[4].AddProduct(products[3], 111);

            Products = products;
            Orders = orders;
        }

        public static ICollection<Product> Products { get; }

        public static ICollection<Order> Orders { get; }

        public static IEnumerable<T> Query<T>()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IEnumerable<T>)Products;
            }

            if (typeof(T) == typeof(Order))
            {
                return (IEnumerable<T>)Orders;
            }

            throw new Exception($"Data type {typeof(T).FullName} may not be served by this data source.");
        }

        private static void AddProduct(this Order order, Product product, int quantity)
            => order.Items.Add(new OrderItem { ProductId = product.Id, UnitPrice = product.Price, Quantity = quantity });

        public static long GetNextProductId()
            => Interlocked.Increment(ref _productCount);

        public static long GetNextOrderId()
            => Interlocked.Increment(ref _orderCount) + 1000;
    }
}