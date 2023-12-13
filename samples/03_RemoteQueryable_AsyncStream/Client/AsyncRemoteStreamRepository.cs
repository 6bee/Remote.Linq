// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Common.SimpleAsyncQueryProtocol;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class AsyncRemoteStreamRepository : IRemoteRepository
    {
        private sealed class AsyncTcpClientEnumerator<T> : IAsyncDisposable
        {
            private readonly Lazy<Task<TcpClient>> _tcpClient;
            private readonly CancellationToken _cancellation;
            private readonly bool _ownsTcpClient;
            private Lazy<T> _current;
            private long _sequence = 0;

            public AsyncTcpClientEnumerator(Func<TcpClient> tcpClientProvider, Expression expression, CancellationToken cancellation, bool ownsTcpClient = true)
            {
                SetError($"{nameof(MoveNextAsync)} has not completed yet.");
                _cancellation = cancellation;
                _ownsTcpClient = ownsTcpClient;
                _tcpClient = new Lazy<Task<TcpClient>>(() => Task.Run(async () =>
                {
                    var tcpClient = tcpClientProvider();
                    var stream = tcpClient.GetStream();
                    await stream.WriteAsync(new AsyncStreamQuery<Expression> { Request = expression }).ConfigureAwait(false);
                    await stream.FlushAsync().ConfigureAwait(false);
                    return tcpClient;
                }));
            }

            private void SetError(string error) => SetError(new InvalidOperationException(error));

            private void SetError(Exception exception)
            {
                _current = new Lazy<T>(() => throw exception);
            }

            private void SetCurrent(T current)
            {
                _current = new Lazy<T>(() => current);
            }

            private T Current => _current.Value;

            private async ValueTask<bool> MoveNextAsync()
            {
                try
                {
                    _cancellation.ThrowIfCancellationRequested();

                    var stream = (await _tcpClient.Value.ConfigureAwait(false)).GetStream();

                    await stream.WriteAsync(new NextRequest { SequenceNumber = Interlocked.Increment(ref _sequence) }).ConfigureAwait(false);
                    await stream.FlushAsync().ConfigureAwait(false);

                    var response = await stream.ReadAsync<NextResponse<T>>().ConfigureAwait(false);
                    if (response.SequenceNumber != _sequence)
                    {
                        var exception = new InvalidOperationException("Async stream is out of bound.");
                        SetError(exception);
                        throw exception;
                    }

                    if (response.HasNext)
                    {
                        SetCurrent(response.Item);
                        return true;
                    }
                    else
                    {
                        SetError("Reached end of stream.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    throw;
                }
            }

            public async ValueTask DisposeAsync()
            {
                if (_ownsTcpClient && _tcpClient.IsValueCreated)
                {
                    await new ValueTask(Task.Run(_tcpClient.Value.Dispose));
                }
            }

            public async IAsyncEnumerable<T> GetAsyncStream()
            {
                try
                {
                    while (await MoveNextAsync().ConfigureAwait(false))
                    {
                        yield return Current;
                    }
                }
                finally
                {
                    await DisposeAsync().ConfigureAwait(false);
                }
            }
        }

        private readonly TcpClient _tcpClient;
        private readonly Func<Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> _asyncStreamDataProvider;

        public AsyncRemoteStreamRepository(string server, int port)
        {
            _tcpClient = new TcpClient(server, port);
            _asyncStreamDataProvider = (expression, cancellation) => new AsyncTcpClientEnumerator<DynamicObject>(() => _tcpClient, expression, cancellation, false).GetAsyncStream();
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncStreamQueryable<ProductCategory>(_asyncStreamDataProvider);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateAsyncStreamQueryable<ProductGroup>(_asyncStreamDataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncStreamQueryable<Product>(_asyncStreamDataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncStreamQueryable<OrderItem>(_asyncStreamDataProvider);

        public void Dispose() => _tcpClient.Dispose();
    }
}