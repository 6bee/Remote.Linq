// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.DataContract
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public long ProductId { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount => Quantity * UnitPrice;

        public override string ToString() => $"Prod #{ProductId}: {Quantity} * {UnitPrice:C} = {TotalAmount:C}";
    }
}
