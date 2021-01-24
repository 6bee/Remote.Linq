// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Client;
    using Server;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Async Stream");
            PrintNote("This demo uses a random delay on server side to simulate data item retrieval/generation via async stream.");
            const string host = "localhost";
            const int port = 8899;

            PrintSetup("Starting TCP service...");
            using var serviceHost = new TcpServer(port);
            serviceHost.RunAsyncStreamQueryService(new QueryService().ExecuteAsyncStreamQuery);

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            new AsyncStreamDemo(() => new AsyncRemoteStreamRepository(host, port)).RunAsync().Wait();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}
