// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using Client;
using Server;
using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("protobuf-net Serialization over TCP/IP");
        const string host = "localhost";
        const int port = 8899;

        PrintSetup("Starting TCP service...");
        using var serviceHost = new TcpServer("0.0.0.0", port);
        serviceHost.RunAsyncQueryService(new QueryService().ExecuteQueryAsync);

        PrintSetup("Staring client demo...");
        PrintSetup("-------------------------------------------------");
        new AsyncDemo(() => new RemoteRepository(host, port)).RunAsyncDemo();

        PrintSetup();
        PrintSetup("-------------------------------------------------");
        PrintSetup("Done.");
        WaitForEnterKey();
    }
}