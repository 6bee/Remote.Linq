// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public sealed class HttpServer : IDisposable
    {
        private readonly HttpListener _server;

        public HttpServer(int port)
        {
            _server = new HttpListener();
            _server.Prefixes.Add($"http://+:{port}/queryservice/");
        }

        public void Open()
        {
            _server.Start();

            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        // Console.Write("Waiting for a connection... ");
                        HttpListenerContext client = _server.GetContext();
                        {
                            // Console.Write("Connected... ");
                            System.IO.Stream inStream = client.Request.InputStream;
                            System.IO.Stream outStream = client.Response.OutputStream;
                            Expression queryExpression = await inStream.ReadAsync<Expression>().ConfigureAwait(false);

                            try
                            {
                                QueryService queryService = new QueryService();
                                IEnumerable<Aqua.Dynamic.DynamicObject> result = queryService.ExecuteQuery(queryExpression);
                                await outStream.WriteAsync(result).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                await outStream.WriteAsync(ex).ConfigureAwait(false);
                            }

                            inStream.Close();
                            outStream.Close();

                            // Console.WriteLine("Closed client session.");
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException: {ex.Message}");
                }
            });
        }

        public void Dispose()
        {
            try
            {
                _server.Stop();
            }
            catch
            {
            }
        }
    }
}
