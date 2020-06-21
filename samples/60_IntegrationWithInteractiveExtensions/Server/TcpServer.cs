// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Common.SimpleAsyncStreamProtocol;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class TcpServer : IDisposable
    {
        private readonly TcpListener _server;

        public TcpServer(int port)
            : this("127.0.0.1", port)
        {
        }

        public TcpServer(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _server = new TcpListener(ipAddress, port);
        }

        public void RunAsyncStreamService<TRequest, TResponse>(Func<TRequest, CancellationToken, IAsyncEnumerable<TResponse>> asyncRequestHandler)
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
                                while (true)
                                {
                                    using var cancellation = new CancellationTokenSource();
                                    var initiate = await stream.ReadAsync<InitializeStream<TRequest>>().ConfigureAwait(false);
                                    var asyncEnumerator = asyncRequestHandler(initiate.Request, cancellation.Token).GetAsyncEnumerator();
                                    try
                                    {
                                        var random = new Random();

                                        var hasNext = true;
                                        while (hasNext)
                                        {
                                            // DEMO: for demo purpose we're using random delay simulating data item generation
                                            await Task.Delay(random.Next(0, 2000)).ConfigureAwait(false);

                                            var next = await stream.ReadAsync<NextRequest>().ConfigureAwait(false);
                                            try
                                            {
                                                hasNext = await asyncEnumerator.MoveNextAsync().ConfigureAwait(false);
                                                var responseNext = new NextResponse<TResponse>
                                                {
                                                    SequenceNumber = next.SequenceNumber,
                                                    HasNext = hasNext,
                                                    Item = hasNext ? asyncEnumerator.Current : default,
                                                };

                                                await stream.WriteAsync(responseNext).ConfigureAwait(false);
                                            }
                                            catch (Exception ex)
                                            {
                                                await stream.WriteAsync(ex).ConfigureAwait(false);
                                            }

                                            await stream.FlushAsync().ConfigureAwait(false);
                                        }
                                    }
                                    finally
                                    {
                                        await asyncEnumerator.DisposeAsync().ConfigureAwait(false);
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

        public void RunAsyncStreamQueryService<TResult>(Func<Expression, CancellationToken, IAsyncEnumerable<TResult>> asyncRequestHandler) => RunAsyncStreamService(asyncRequestHandler);

        public void Dispose() => _server.Stop();
    }
}
