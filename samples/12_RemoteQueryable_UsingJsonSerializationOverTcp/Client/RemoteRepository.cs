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

    public class RemoteRepository : IRemoteRepository
    {
        private readonly TcpClient _tcpClient;
        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

        public RemoteRepository(string server, int port)
        {
            _tcpClient = new TcpClient(server, port);
            _dataProvider = expression =>
            {
                var stream = _tcpClient.GetStream();

                stream.Write(expression);

                IEnumerable<DynamicObject> result = stream.Read<IEnumerable<DynamicObject>>();
                return result;
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ProductGroup>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);

        public void Dispose() => _tcpClient.Dispose();
    }
}
