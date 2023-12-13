// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable.QueryTestData
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Store
    {
        private readonly IDictionary<Type, IEnumerable> _dataSets;

        public Store()
        {
            _dataSets = new Dictionary<Type, IEnumerable>();

            _dataSets[typeof(Category)] = new[]
            {
                new Category { Id = 1, Name = "Fruits", CategoryType = CategoryType.Food, CategorySourceType = CategorySourceType.Internal },
                new Category { Id = 2, Name = "Vehicles", CategoryType = CategoryType.NonFood, CategorySourceType = null },
            };

            _dataSets[typeof(Product)] = new[]
            {
                new Product { Id = 10, Name = "Apple", Price = .8m, CategoryId = 1 },
                new Product { Id = 11, Name = "Pear", Price = 1.15m, CategoryId = 1 },
                new Product { Id = 12, Name = "Car", Price = 33999m, CategoryId = 2 },
                new Product { Id = 13, Name = "Pineapple", Price = 2.99m, CategoryId = 1 },
                new Product { Id = 14, Name = "Bicycle", Price = 149.95m, CategoryId = 2, PruductTags = PruductTags.BestPrice | PruductTags.TopSelling },
            };

            _dataSets[typeof(OrderItem)] = new[]
            {
                new OrderItem { Id = 1000, OrderId = 100, ProductId = 10, Quantity = 2 },
                new OrderItem { Id = 1001, OrderId = 101, ProductId = 11, Quantity = 3 },
                new OrderItem { Id = 1002, OrderId = 101, ProductId = 14, Quantity = 4 },
            };

            _dataSets[typeof(Order)] = new[]
            {
                new Order
                {
                    Id = 100,
                    Items = _dataSets[typeof(OrderItem)].OfType<OrderItem>().Where(x => x.OrderId == 100).ToList(),
                    ShippingAddress = null,
                },
                new Order
                {
                    Id = 101,
                    Items = _dataSets[typeof(OrderItem)].OfType<OrderItem>().Where(x => x.OrderId == 101).ToList(),
                    ShippingAddress = new Address
                    {
                        Street = "Pennsylvania Ave",
                        StreetNumber = "1600",
                        City = "Washington, DC",
                        ZipCode = "20500",
                    },
                },
            };

            _dataSets[typeof(Address)] = _dataSets[typeof(Order)]
                .OfType<Order>()
                .Select(x => x.ShippingAddress)
                .Distinct()
                .ToArray();
        }

        public IQueryable<T> Get<T>() => ((IEnumerable<T>)_dataSets[typeof(T)]).AsQueryable();

        public IQueryable Get(Type type) => _dataSets[type].AsQueryable();
    }
}