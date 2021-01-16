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
    using System.Threading.Tasks;

    public partial class RemoteRepository : IRemoteRepository
    {
        private readonly TcpClient _tcpClient;
        private readonly Func<Expression, ValueTask<IEnumerable<DynamicObject>>> _asyncDataProvider;

        public RemoteRepository(string server, int port)
        {
            _tcpClient = new TcpClient(server, port);
            _asyncDataProvider = async expression =>
            {
                var stream = _tcpClient.GetStream();

                await stream.WriteAsync(expression).ConfigureAwait(false);
                await stream.FlushAsync().ConfigureAwait(false);

                var result = await stream.ReadAsync<IEnumerable<DynamicObject>>().ConfigureAwait(false);
                return result;
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_asyncDataProvider);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ProductGroup>(_asyncDataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_asyncDataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_asyncDataProvider);

        public void Dispose() => _tcpClient.Dispose();
    }
}
