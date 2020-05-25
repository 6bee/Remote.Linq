// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;

    public sealed class RemoteRepository : IDisposable
    {
        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;
        private readonly TcpClient _client;

        public RemoteRepository(string server, int port)
        {
            _client = new TcpClient(server, port);
            _dataProvider = expression =>
            {
                var stream = _client.GetStream();

                stream.Write(expression);

                var result = stream.Read<IEnumerable<DynamicObject>>();

                return result;
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
