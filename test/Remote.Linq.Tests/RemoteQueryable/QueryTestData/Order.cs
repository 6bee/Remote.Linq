// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable.QueryTestData;

using System.Collections.Generic;

public class Order
{
    public int Id { get; set; }

    public Address ShippingAddress { get; set; }

    public ICollection<OrderItem> Items { get; set; }
}