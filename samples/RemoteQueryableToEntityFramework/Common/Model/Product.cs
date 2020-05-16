// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    using System.Collections.Generic;

    public class Product
    {
        public int Id { get; set; }

        public ProductCategory ProductCategory { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public IList<Market> Markets { get; set; }
    }
}
