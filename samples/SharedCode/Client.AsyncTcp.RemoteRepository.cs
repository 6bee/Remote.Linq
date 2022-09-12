// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class RemoteRepository : IRemoteRepository
    {
        private readonly TcpClient _tcpClient;
        private readonly Func<Expression, CancellationToken, ValueTask<DynamicObject>> _asyncDataProvider;

        public RemoteRepository(string server, int port)
        {
            _tcpClient = new TcpClient(server, port);
            _asyncDataProvider = async (expression, cancellation) =>
            {
                var stream = _tcpClient.GetStream();

                await stream.WriteAsync(expression, cancellation).ConfigureAwait(false);
                await stream.FlushAsync(cancellation).ConfigureAwait(false);

                var result = await stream.ReadAsync<DynamicObject>(cancellation).ConfigureAwait(false);
                return result;
            };
        }

        ////public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncQueryable<ProductCategory>(_asyncDataProvider);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateAsyncQueryable<ProductGroup>(_asyncDataProvider);

        ////public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncQueryable<Product>(_asyncDataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncQueryable<OrderItem>(_asyncDataProvider);

        public void Dispose() => _tcpClient.Dispose();
    }
}
