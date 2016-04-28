// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client.Model
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
