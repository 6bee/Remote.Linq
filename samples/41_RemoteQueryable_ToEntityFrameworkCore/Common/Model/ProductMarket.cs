// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public partial class ProductMarket
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int MarketId { get; set; }

        public Market Market { get; set; }
    }
}
