// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.DataContract
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class Order
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);

        public override string ToString()
            => string.Format("Order: {0} Item{2}  Total {1:C}", Items.Count, TotalAmount, Items.Count > 1 ? "s" : null);
    }
}
