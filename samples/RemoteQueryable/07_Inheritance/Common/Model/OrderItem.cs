// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public class OrderItem : Entity
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
