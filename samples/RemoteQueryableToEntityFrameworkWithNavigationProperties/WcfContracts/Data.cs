// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfContracts
{
    using System.Collections.Generic;

    public class OrderItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public ProductCategory ProductCategory { get; set; }
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
