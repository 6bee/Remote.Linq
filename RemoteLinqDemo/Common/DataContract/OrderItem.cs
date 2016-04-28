// Copyright (c) Christof Senn. All rights reserved. 

using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class OrderItem
    {
        [DataMember]
        public long ProductId { get; set; }

        [DataMember]
        public int Quantity { get; set; }

        [DataMember]
        public decimal UnitPrice { get; set; }

        public decimal TotalAmount
        {
            get { return Quantity * UnitPrice; }
        }

        public override string ToString()
        {
            return string.Format("Prod #{0}: {1} * {2:C} = {3:C}", ProductId, Quantity, UnitPrice, TotalAmount);
        }
    }
}
