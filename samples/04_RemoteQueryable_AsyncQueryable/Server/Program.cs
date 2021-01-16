// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Async Stream [Server]");
            PrintNote("This demo uses a random delay on server side to simulate data item retrieval/generation.");
            using var serviceHost = new TcpServer(8899);
            serviceHost.RunAsyncStreamQueryService(new AsyncQueryService().ExecuteAsyncStreamQuery);

            PrintServerReady();
            WaitForEnterKey();
        }
    }
}
