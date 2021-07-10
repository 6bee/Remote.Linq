// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Client;
    using Grpc.Core;
    using Server;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            ParseContextArgs(args);
            Title("ServiceModel gRPC");
            const string host = "localhost";
            const int port = 8899;

            PrintSetup("Starting TCP service...");
            var server = new Server();
            server.Ports.Add(new ServerPort("localhost", 8899, ServerCredentials.Insecure));
            server.Services.AddServiceModelTransient(() => new QueryService());
            server.Start();

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            new AsyncDemo(() => new RemoteRepository(host, port)).RunAsyncDemo();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}