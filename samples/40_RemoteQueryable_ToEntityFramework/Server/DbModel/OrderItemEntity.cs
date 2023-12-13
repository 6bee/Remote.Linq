// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server.DbModel
{
    public class OrderItemEntity
    {
        public int Id { get; set; }

        public ProductEntity Product { get; set; }

        public int Quantity { get; set; }
    }
}