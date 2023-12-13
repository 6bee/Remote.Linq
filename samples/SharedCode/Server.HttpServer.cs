// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Common;
using Remote.Linq.Expressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public sealed class HttpServer : IDisposable
{
    private readonly HttpListener _server;

    public HttpServer(int port)
    {
        _server = new HttpListener();
        _server.Prefixes.Add($"http://+:{port}/queryservice/");
    }

    public void RunService<TRequest, TResponse>(Func<TRequest, CancellationToken, ValueTask<TResponse>> asyncRequestHandler)
    {
        _server.Start();

        _ = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    HttpListenerContext client = await _server.GetContextAsync().ConfigureAwait(false);
                    using var inStream = client.Request.InputStream;
                    using var outStream = client.Response.OutputStream;

                    var request = await inStream.ReadAsync<TRequest>().ConfigureAwait(false);
                    try
                    {
                        var response = await asyncRequestHandler(request, CancellationToken.None).ConfigureAwait(false);
                        await outStream.WriteAsync(response).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await outStream.WriteAsync(ex).ConfigureAwait(false);
                    }

                    await outStream.FlushAsync().ConfigureAwait(false);
                }
            }
            catch (SocketException)
            {
                // tcp server terminated
            }
        });
    }

    public void RunQueryService<TResult>(Func<Expression, CancellationToken, ValueTask<TResult>> asyncRequestHandler) => RunService(asyncRequestHandler);

    public void Dispose() => _server.Stop();
}