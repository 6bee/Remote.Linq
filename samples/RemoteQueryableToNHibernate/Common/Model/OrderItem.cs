// Copyright (c) Christof Senn. All rights reserved. 

namespace Common.Model
{
    public class OrderItem
    {
        public virtual int Id { get; set; }
        public virtual int ProductId { get; set; }
        public virtual int Quantity { get; set; }
    }
}
