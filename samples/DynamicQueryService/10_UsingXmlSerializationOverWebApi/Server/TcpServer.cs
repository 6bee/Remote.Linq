// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common.Helpers;
using Remote.Linq.Expressions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class TcpServer : IDisposable
    {
        private readonly TcpListener _server;

        public TcpServer(string ip, int port)
        {
            var ipAddress = IPAddress.Parse(ip);
            _server = new TcpListener(ipAddress, port);
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

                        using (var client = _server.AcceptTcpClient())
                        {
                            Console.Write("Connected... ");

                            using (var stream = client.GetStream())
                            {
                                var queryExpression = stream.Read<Expression>();

                                try
                                {
                                    var queryService = new QueryService();
                                    var result = queryService.ExecuteQuery(queryExpression);
                                    stream.Write(result);
                                }
                                catch (Exception ex)
                                {
                                    stream.Write(ex);
                                }

                                stream.Close();
                            }

                            client.Close();
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
