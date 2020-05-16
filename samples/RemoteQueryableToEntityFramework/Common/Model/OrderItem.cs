// Copyright (c) Christof Senn. All rights reserved. 

namespace Common.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
