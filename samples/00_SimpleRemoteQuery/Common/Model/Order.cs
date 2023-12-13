// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class Order
    {
        public long Id { get; set; }

        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

        public void Add(Product product, int quantity)
            => Items.Add(new OrderItem { ProductId = product.Id, UnitPrice = product.Price, Quantity = quantity });
    }
}