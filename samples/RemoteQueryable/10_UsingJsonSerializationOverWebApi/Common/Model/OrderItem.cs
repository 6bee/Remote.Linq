// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public class OrderItem
    {
        public long Id { get; set; }

        public long ProductId { get; set; }

        public long Quantity { get; set; }
    }
}
