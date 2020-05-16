// Copyright (c) Christof Senn. All rights reserved. 

namespace Common.Model
{
    using System.Collections.Generic;

    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
