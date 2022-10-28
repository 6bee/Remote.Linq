﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    using System.Collections.Generic;

    public partial class ProductGroup
    {
        public string Id { get; set; }

        public string GroupName { get; set; }

        public IList<Product> Products { get; set; }
    }
}
