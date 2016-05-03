// Copyright (c) Christof Senn. All rights reserved. 

namespace Common.Model
{
    public class Product
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
