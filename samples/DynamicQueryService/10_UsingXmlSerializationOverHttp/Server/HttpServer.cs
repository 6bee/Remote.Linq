// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common;
using Remote.Linq.Expressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener _server;

        public HttpServer(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _server = new HttpListener();
            _server.Prefixes.Add(string.Format("http://{0}:{1}/queryservice/", ip, port));
        }

        public void Open()
        {
            _server.Start();

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        Console.Write("Waiting for a connection... ");

                        var client = _server.GetContext();
                        {
                            Console.Write("Connected... ");

                            var inStream = client.Request.InputStream;
                            var outStream = client.Response.OutputStream;
                            {
                                var queryExpression = inStream.Read<Expression>();

                                try
                                {
                                    var queryService = new QueryService();
                                    var result = queryService.ExecuteQuery(queryExpression);
                                    outStream.Write(result);
                                }
                                catch (Exception ex)
                                {
                                    outStream.Write(ex);
                                }

                                inStream.Close();
                                outStream.Close();
                            }

                            Console.WriteLine("Closed client session.");
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: {0}", ex);
                }
            });
        }

        public void Dispose()
        {
            try
            {
                _server.Stop();
            }
            catch { }
        }
    }
}
