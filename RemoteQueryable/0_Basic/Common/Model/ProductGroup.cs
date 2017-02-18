// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
   using System.Collections.Generic;

    public class ProductGroup
    {
        public int Id { get; set; }

        public string GroupName { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
