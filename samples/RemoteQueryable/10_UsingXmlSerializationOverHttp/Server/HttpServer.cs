// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Remote.Linq.Expressions;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class HttpServer : IDisposable
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
                        //Console.Write("Waiting for a connection... ");

                        var client = _server.GetContext();
                        {
                            //Console.Write("Connected... ");

                            var inStream = client.Request.InputStream;
                            var outStream = client.Response.OutputStream;
                            {
                                var queryExpression = await inStream.ReadAsync<Expression>();

                                try
                                {
                                    var queryService = new QueryService();
                                    var result = queryService.ExecuteQuery(queryExpression);
                                    await outStream.WriteAsync(result);
                                }
                                catch (Exception ex)
                                {
                                    await outStream.WriteAsync(ex);
                                }

                                inStream.Close();
                                outStream.Close();
                            }

                            //Console.WriteLine("Closed client session.");
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
