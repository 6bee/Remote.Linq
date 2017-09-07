// Copyright (c) Christof Senn. All rights reserved. 

namespace Common.Model
{
    using System.Collections.Generic;

    public class Market
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Product> Products { get; set; }
    }
}
