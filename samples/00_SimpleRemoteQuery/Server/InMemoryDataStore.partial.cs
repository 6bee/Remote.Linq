// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common.Model;

partial class InMemoryDataStore
{
    private Order[] _orders;

    partial void OnInitialize()
    {
        _orders = new[]
        {
            new Order { Id = 1000 },
            new Order { Id = 1001 },
            new Order { Id = 1002 },
            new Order { Id = 1003 },
            new Order { Id = 1004 },
        };

        _orders[0].Add(_products[0], 100);
        _orders[0].Add(_products[1], 20);
        _orders[0].Add(_products[2], 1);
        _orders[1].Add(_products[0], 8000);
        _orders[1].Add(_products[2], 2);
        _orders[2].Add(_products[1], 1);
        _orders[4].Add(_products[1], 1);
        _orders[4].Add(_products[2], 11);
        _orders[4].Add(_products[3], 111);
    }

    public IEnumerable<Order> Orders => _orders.ToArray();
}