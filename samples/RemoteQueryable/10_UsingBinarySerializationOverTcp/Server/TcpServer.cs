// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Remote.Linq.Expressions;
    using System;
    using System.Net;
    using System.Net.Sockets;
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
            IPAddress ipAddress = IPAddress.Parse(ip);
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
                        using (TcpClient client = _server.AcceptTcpClient())
                        {
                            using (NetworkStream stream = client.GetStream())
                            {
                                Expression queryExpression = stream.Read<Expression>();

                                try
                                {
                                    QueryService queryService = new QueryService();
                                    System.Collections.Generic.IEnumerable<Aqua.Dynamic.DynamicObject> result = queryService.ExecuteQuery(queryExpression);
                                    stream.Write(result);
                                }
                                catch (Exception ex)
                                {
                                    stream.Write(ex);
                                }

                                stream.Close();
                            }

                            client.Close();
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
                // ignore
            }
        }
    }
}
