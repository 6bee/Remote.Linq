// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.DataContract
{
    using System.Runtime.Serialization;

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
