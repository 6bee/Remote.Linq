﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("Binary Serialization over TCP/IP async");
        const string host = "localhost";
        const int port = 8899;

        PrintSetup("Starting TCP service...");
        using var serviceHost = new Server.TcpServer(port);
        serviceHost.RunAsyncQueryService(new Server.QueryService().ExecuteQueryAsync);

        PrintSetup("Starting client demo...");
        PrintSetup("-------------------------------------------------");
        new Client.AsyncDemo(() => new Client.RemoteRepository(host, port)).RunAsyncDemo();

        PrintSetup();
        PrintSetup("-------------------------------------------------");
        PrintSetup("Done.");
        WaitForEnterKey();
    }
}