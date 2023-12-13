// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Grpc.Core;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("ServiceModel gRPC [Server]");

            var server = new Server();
            server.Ports.Add(new ServerPort("localhost", 8899, ServerCredentials.Insecure));

            server.Services.AddServiceModelTransient(() => new QueryService());

            server.Start();

            PrintServerReady();
            WaitForEnterKey();
        }
    }
}