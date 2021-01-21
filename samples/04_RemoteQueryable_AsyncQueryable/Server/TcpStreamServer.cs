// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Common.SimpleAsyncQueryProtocol;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class TcpStreamServer : IDisposable
    {
        private readonly TcpListener _server;

        public TcpStreamServer(int port)
            : this("127.0.0.1", port)
        {
        }

        public TcpStreamServer(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _server = new TcpListener(ipAddress, port);
        }

        public void RunAsyncStreamQueryService(
            Func<Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> asyncStreamRequestHandler,
            Func<Expression, CancellationToken, ValueTask<DynamicObject>> asyncQueryRequestHandler)
            => RunAsyncStreamService(asyncStreamRequestHandler, asyncQueryRequestHandler);

        public void RunAsyncStreamService<TRequest, TStreamResponse, TQueryResponse>(
            Func<TRequest, CancellationToken, IAsyncEnumerable<TStreamResponse>> asyncStreamRequestHandler,
            Func<TRequest, CancellationToken, ValueTask<TQueryResponse>> asyncQueryRequestHandler)
        {
            _server.Start();

            _ = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        TcpClient tcpClient = await _server.AcceptTcpClientAsync().ConfigureAwait(false);
                        _ = Task.Run(async () =>
                        {
                            using var client = tcpClient;
                            try
                            {
                                using var stream = client.GetStream();
                                using var cancellation = new CancellationTokenSource();
                                while (true)
                                {
                                    var query = await stream.ReadAsync<IQuery<TRequest>>(cancellation.Token).ConfigureAwait(false);
                                    if (query is AsyncStreamQuery<TRequest>)
                                    {
                                        await ExecuteQueryAsAsyncStreamOperation(query.Request, asyncStreamRequestHandler, stream, cancellation.Token).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        await ExecuteQueryAsAsyncBatchOperation(query.Request, asyncQueryRequestHandler, stream, cancellation.Token).ConfigureAwait(false);
                                    }
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // client sesstion terminated
                                client.Close();
                            }
                        });
                    }
                }
                catch (SocketException)
                {
                    // tcp server terminated
                }
            });
        }

        private static async Task ExecuteQueryAsAsyncBatchOperation<TRequest, TQueryResponse>(TRequest request, Func<TRequest, CancellationToken, ValueTask<TQueryResponse>> asyncQueryRequestHandler, NetworkStream stream, CancellationToken cancellation)
        {
            try
            {
                var response = await asyncQueryRequestHandler(request, cancellation).ConfigureAwait(false);
                await stream.WriteAsync(response, cancellation).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                await stream.WriteAsync(ex, cancellation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await stream.WriteAsync(new Exception($"{ex.GetType()}: {ex.Message}"), cancellation).ConfigureAwait(false);
            }

            await stream.FlushAsync(cancellation).ConfigureAwait(false);
        }

        private static async Task ExecuteQueryAsAsyncStreamOperation<TRequest, TStreamResponse>(TRequest request, Func<TRequest, CancellationToken, IAsyncEnumerable<TStreamResponse>> asyncStreamRequestHandler, NetworkStream stream, CancellationToken cancellation)
        {
            var asyncEnumerator = asyncStreamRequestHandler(request, cancellation).GetAsyncEnumerator();
            try
            {
                var random = new Random();

                var hasNext = true;
                while (hasNext)
                {
                    // DEMO: for demo purpose we're using random delay simulating data item generation
                    await Task.Delay(random.Next(0, 2000), cancellation).ConfigureAwait(false);

                    var next = await stream.ReadAsync<NextRequest>(cancellation).ConfigureAwait(false);
                    try
                    {
                        hasNext = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                        var responseNext = new NextResponse<TStreamResponse>
                        {
                            SequenceNumber = next.SequenceNumber,
                            HasNext = hasNext,
                            Item = hasNext ? asyncEnumerator.Current : default,
                        };

                        await stream.WriteAsync(responseNext, cancellation).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await stream.WriteAsync(ex, cancellation).ConfigureAwait(false);
                    }

                    await stream.FlushAsync(cancellation).ConfigureAwait(false);
                }
            }
            finally
            {
                await asyncEnumerator.DisposeAsync().ConfigureAwait(false);
            }
        }

        public void Dispose() => _server.Stop();
    }
}
