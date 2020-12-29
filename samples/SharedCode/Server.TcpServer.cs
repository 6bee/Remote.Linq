// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Remote.Linq.Expressions;
    using System;
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

        public void RunService<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler)
            => RunAsyncService<TRequest, TResponse>((request, cancellation) => Task.FromResult(requestHandler(request)));

        public void RunAsyncService<TRequest, TResponse>(Func<TRequest, CancellationToken, Task<TResponse>> asyncRequestHandler)
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
                                    var request = await stream.ReadAsync<TRequest>().ConfigureAwait(false);
                                    try
                                    {
                                        var response = await asyncRequestHandler(request, cancellation.Token).ConfigureAwait(false);
                                        await stream.WriteAsync(response).ConfigureAwait(false);
                                    }
                                    catch (InvalidOperationException ex)
                                    {
                                        await stream.WriteAsync(ex).ConfigureAwait(false);
                                    }
                                    catch (Exception ex)
                                    {
                                        await stream.WriteAsync(new Exception($"{ex.GetType()}: {ex.Message}")).ConfigureAwait(false);
                                    }

                                    await stream.FlushAsync().ConfigureAwait(false);
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

        public void RunQueryService<TResult>(Func<Expression, TResult> requestHandler) => RunService(requestHandler);

        public void RunAsyncQueryService<TResult>(Func<Expression, CancellationToken, Task<TResult>> asyncRequestHandler) => RunAsyncService(asyncRequestHandler);

        public void Dispose() => _server.Stop();
    }
}
