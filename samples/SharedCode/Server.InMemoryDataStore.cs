// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.Model;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public sealed partial class InMemoryDataStore
    {
        private readonly ProductGroup[] _productGroups;
        private readonly ProductCategory[] _productCategories;
        private readonly Product[] _products;
        private readonly OrderItem[] _orderItems;

        private InMemoryDataStore()
        {
            _productCategories = new[]
            {
                new ProductCategory { Id = 1, Name = "Fruits" },
                new ProductCategory { Id = 2, Name = "Vehicles" },
            };

            _products = new[]
            {
                new Product { Id = 10, Name = "Apple", Price = 1m, ProductCategoryId = 1 },
                new Product { Id = 11, Name = "Pear", Price = 2m, ProductCategoryId = 1 },
                new Product { Id = 12, Name = "Pineapple", Price = 3m, ProductCategoryId = 1 },
                new Product { Id = 13, Name = "Car", Price = 33999m, ProductCategoryId = 2 },
                new Product { Id = 14, Name = "Bicycle", Price = 150m, ProductCategoryId = 2 },
            };

            _orderItems = new[]
            {
                new OrderItem { Id = 100, ProductId = 10, Quantity = 2 },
                new OrderItem { Id = 101, ProductId = 11, Quantity = 3 },
                new OrderItem { Id = 102, ProductId = 14, Quantity = 4 },
            };

            foreach (var orderItem in _orderItems)
            {
                orderItem.UnitPrice = _products.Single(x => x.Id == orderItem.ProductId).Price;
            }

            _productGroups = new[]
            {
                new ProductGroup
                {
                    Id = 0,
                    GroupName = "All",
                    Products = _products.ToList(),
                },
                new ProductGroup
                {
                    Id = 1,
                    GroupName = "Food",
                    Products = _products.Where(x => x.ProductCategoryId == 1).ToList(),
                },
                new ProductGroup
                {
                    Id = 2,
                    GroupName = "NonFood",
                    Products = _products.Where(x => x.ProductCategoryId == 2).ToList(),
                },
            };

            OnInitialize();
        }

        partial void OnInitialize();

        public static InMemoryDataStore Instance { get; } = new InMemoryDataStore();

        public IQueryable<ProductGroup> ProductGroups => _productGroups.AsQueryable();

        public IQueryable<ProductCategory> ProductCategories => _productCategories.AsQueryable();

        public IQueryable<Product> Products => _products.AsQueryable();

        public IQueryable<OrderItem> OrderItems => _orderItems.AsQueryable();

        public Func<Type, IQueryable> QueryableByTypeProvider => (Type type) =>
        {
            // return wellknown entity sets
            if (type == typeof(ProductGroup))
            {
                return ProductGroups;
            }

            if (type == typeof(ProductCategory))
            {
                return ProductCategories;
            }

            if (type == typeof(Product))
            {
                return Products;
            }

            if (type == typeof(OrderItem))
            {
                return OrderItems;
            }

            // return entity sets possibly declared in partial class
            var queryableType = typeof(IEnumerable<>).MakeGenericType(type);
            var dataset = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => queryableType.IsAssignableFrom(p.PropertyType))
                .FirstOrDefault()?
                .GetValue(this) as IEnumerable;
            return dataset?.AsQueryable() ?? throw new NotSupportedException($"No queryable resource available for type {type}");
        };
    }
}