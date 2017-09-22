// Copyright (c) Christof Senn. All rights reserved. 

using System.Runtime.Serialization;

namespace Common.DataContract
{
    [DataContract]
    public class Product
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        public override string ToString()
            => string.Format("Product #{0} '{1}' ({2:C})", Id, Name, Price);
    }
}
