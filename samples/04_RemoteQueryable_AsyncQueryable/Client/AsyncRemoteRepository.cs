// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Common.SimpleAsyncStreamProtocol;
    using Remote.Linq;
    using Remote.Linq.Async.Queryable;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class AsyncRemoteRepository : IAsyncRemoteRepository
    {
        private sealed class AsyncTcpQueryClient : IAsyncDisposable
        {
            private readonly string _server;
            private readonly int _port;

            public AsyncTcpQueryClient(string server, int port)
            {
                _server = server;
                _port = port;
            }

            public async ValueTask<DynamicObject> ExecuteAsync(Expression expression, CancellationToken cancellation)
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(_server, _port, cancellation);

                using var stream = tcpClient.GetStream();
                await stream.WriteAsync(new AsyncQuery<Expression> { Request = expression }, cancellation).ConfigureAwait(false);
                await stream.FlushAsync(cancellation).ConfigureAwait(false);

                var result = await stream.ReadAsync<DynamicObject>(cancellation).ConfigureAwait(false);
                return result;
            }

            public async IAsyncEnumerable<DynamicObject> ExecuteAsyncStream(Expression expression, [EnumeratorCancellation] CancellationToken cancellation)
            {
                using var tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(_server, _port, cancellation);

                await using var asyncTcpClientEnumerator = new AsyncTcpStreamEnumerator<DynamicObject>(tcpClient, expression, cancellation);

                while (await asyncTcpClientEnumerator.MoveNextAsync().ConfigureAwait(false))
                {
                    yield return asyncTcpClientEnumerator.Current;
                }
            }

            public async ValueTask DisposeAsync() => await Task.Yield();
        }

        private sealed class AsyncTcpStreamEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly Lazy<Task<TcpClient>> _tcpClient;
            private readonly CancellationToken _cancellation;
            private Lazy<T> _current;
            private long _sequence = 0;

            public AsyncTcpStreamEnumerator(TcpClient tcpClient, Expression expression, CancellationToken cancellation)
            {
                SetError($"{nameof(MoveNextAsync)} has not completed yet.");
                _cancellation = cancellation;
                _tcpClient = new Lazy<Task<TcpClient>>(() => Task.Run(async () =>
                {
                    var stream = tcpClient.GetStream();
                    await stream.WriteAsync(new AsyncStreamQuery<Expression> { Request = expression }, cancellation).ConfigureAwait(false);
                    await stream.FlushAsync(cancellation).ConfigureAwait(false);
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

            public T Current => _current.Value;

            public async ValueTask DisposeAsync()
            {
                if (_tcpClient.IsValueCreated)
                {
                    await Task.Run(_tcpClient.Value.Dispose).ConfigureAwait(false);
                }
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                try
                {
                    _cancellation.ThrowIfCancellationRequested();

                    var stream = (await _tcpClient.Value.ConfigureAwait(false)).GetStream();

                    await stream.WriteAsync(new NextRequest { SequenceNumber = Interlocked.Increment(ref _sequence) }, _cancellation).ConfigureAwait(false);
                    await stream.FlushAsync(_cancellation).ConfigureAwait(false);

                    var response = await stream.ReadAsync<NextResponse<T>>(_cancellation).ConfigureAwait(false);
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
        }

        private readonly AsyncTcpQueryClient _client;

        public AsyncRemoteRepository(string server, int port)
        {
            _client = new AsyncTcpQueryClient(server, port);
        }

        public IAsyncQueryable<ProductCategory> ProductCategories => CreateAsyncQueryable<ProductCategory>();

        public IAsyncQueryable<ProductGroup> ProductGroups => CreateAsyncQueryable<ProductGroup>();

        public IAsyncQueryable<Product> Products => CreateAsyncQueryable<Product>();

        public IAsyncQueryable<OrderItem> OrderItems => CreateAsyncQueryable<OrderItem>();

        public ValueTask DisposeAsync() => _client.DisposeAsync();

        // NOTE: One of `asyncStreamProvider` or `asyncDataProvider` parameter may also be set null in which case all queries are always fetched via stream or via batch query accordingly.
        private IAsyncQueryable<T> CreateAsyncQueryable<T>() => RemoteQueryable.Factory.CreateAsyncQueryable<T>(_client.ExecuteAsyncStream, _client.ExecuteAsync);
    }
}
