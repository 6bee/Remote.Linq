// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
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
                        TcpClient client = _server.AcceptTcpClient();
                        Task.Run(() =>
                        {
                            try
                            {
                                NetworkStream stream = client.GetStream();
                                while (true)
                                {
                                    Expression queryExpression = stream.Read<Expression>();

                                    try
                                    {
                                        QueryService queryService = new QueryService();
                                        IEnumerable<DynamicObject> result = queryService.ExecuteQuery(queryExpression);
                                        stream.Write(result);
                                    }
                                    catch (Exception ex)
                                    {
                                        stream.Write(ex);
                                    }
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // client sesstion terminated
                                client.Dispose();
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
