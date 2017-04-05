// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq.Tests.RemoteQueryable.QueryTestData
{
    public enum CategoryType
    {
        Undefined,
        Food,
        NonFood,
    }

    [Flags]
    public enum PruductTags
    {
        None = 0,
        BestPrice = 1,
        TopSelling = 2,
        Premium = 4,
    }

    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // TODO: add nullable enum property
        public CategoryType CategoryType { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public PruductTags PruductTags { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
