// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public partial class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}