// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    public class OrderItem
    {
        public virtual int Id { get; set; }

        public virtual int ProductId { get; set; }

        public virtual int Quantity { get; set; }
    }
}
