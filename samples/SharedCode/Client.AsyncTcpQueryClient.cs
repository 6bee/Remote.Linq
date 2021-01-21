// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.SimpleAsyncQueryProtocol;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class AsyncTcpQueryClient : IAsyncDisposable
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
            var request = new AsyncQuery<Expression> { Request = expression };
            using var tcpClient = await InitiateQueryAsync(request, cancellation).ConfigureAwait(false);

            using var stream = tcpClient.GetStream();
            var result = await stream.ReadAsync<DynamicObject>(cancellation).ConfigureAwait(false);
            return result;
        }

        public async IAsyncEnumerable<DynamicObject> ExecuteAsyncStream(Expression expression, [EnumeratorCancellation] CancellationToken cancellation)
        {
            var request = new AsyncStreamQuery<Expression> { Request = expression };
            var tcpClient = await InitiateQueryAsync(request, cancellation).ConfigureAwait(false);

            using var stream = tcpClient.GetStream();
            await using var asyncTcpClientEnumerator = new AsyncStreamEnumerator<DynamicObject>(stream, cancellation);
            while (await asyncTcpClientEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return asyncTcpClientEnumerator.Current;
            }
        }

        private async ValueTask<TcpClient> InitiateQueryAsync(IQuery<Expression> request, CancellationToken cancellation)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_server, _port, cancellation);

            var stream = tcpClient.GetStream();
            await stream.WriteAsync(request, cancellation).ConfigureAwait(false);
            await stream.FlushAsync(cancellation).ConfigureAwait(false);

            return tcpClient;
        }

        public async ValueTask DisposeAsync() => await Task.Yield();
    }
}