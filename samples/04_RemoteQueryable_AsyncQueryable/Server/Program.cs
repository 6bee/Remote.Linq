// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using static CommonHelper;

internal static class Program
{
    private static void Main()
    {
        Title("Ix.NET [Server]");
        PrintNote("This demo uses a random delay on server side to simulate data item retrieval/generation via async stream.");
        using var asyncStreamServiceHost = new TcpStreamServer(8899);
        var asyncQueryService = new AsyncQueryService();
        asyncStreamServiceHost.RunAsyncStreamQueryService(asyncQueryService.ExecuteAsyncStreamQuery, asyncQueryService.ExecuteQueryAsync);

        PrintServerReady();
        WaitForEnterKey();
    }
}