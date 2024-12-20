﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using Client;
using Server;
using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("Ix.NET");
        PrintNote("This demo uses a random delay on server side to simulate data item retrieval/generation via async stream.");
        const string host = "localhost";
        const int port = 8899;

        PrintSetup("Starting TCP service...");
        using var asyncStreamServiceHost = new TcpStreamServer(port);
        var asyncQueryService = new AsyncQueryService();
        asyncStreamServiceHost.RunAsyncStreamQueryService(asyncQueryService.ExecuteAsyncStreamQuery, asyncQueryService.ExecuteQueryAsync);

        PrintSetup("Starting client demo...");
        PrintSetup("-------------------------------------------------");
        new AsyncStreamDemo(() => new AsyncTcpRemoteRepository(host, port)).RunAsyncDemo();

        PrintSetup();
        PrintSetup("-------------------------------------------------");
        PrintSetup("Done.");
        WaitForEnterKey();
    }
}