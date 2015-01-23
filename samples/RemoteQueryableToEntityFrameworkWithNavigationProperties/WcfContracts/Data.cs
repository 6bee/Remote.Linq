// Copyright (c) Christof Senn. All rights reserved. 

using System.Collections.Generic;

namespace WcfContracts
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public IList<Market> Markets { get; set; }
    }

    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Market
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Product> Products { get; set; }
    }
}
