// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Remote.Linq.Expressions;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Security;
    using System.Threading.Tasks;

    public sealed class Server : MarshalByRefObject, IDisposable
    {
        private readonly TcpListener _server;

        public Server(string ip, int port)
        {
            _server = new TcpListener(IPAddress.Parse(ip), port);
        }

        public void Start()
        {
            _server.Start();
            _ = Task.Run(AcceptClient);
        }

        [SecuritySafeCritical]
        private void AcceptClient()
        {
            while (true)
            {
                using (var client = _server.AcceptTcpClient())
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
                    client.Close();
                }
            }
        }

        public void Stop() => _server.Stop();

        public void Dispose() => Stop();
    }
}
