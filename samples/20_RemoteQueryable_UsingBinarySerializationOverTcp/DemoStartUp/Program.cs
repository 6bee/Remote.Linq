// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("Binary Serialization over TCP/IP");
        const string host = "localhost";
        const int port = 8899;

        PrintSetup("Starting TCP service...");

        using var tcpServiceHost = new Server.TcpServer(port);
        tcpServiceHost.RunQueryService(new Server.QueryService().ExecuteQuery);

        PrintSetup("Staring client demo...");
        PrintSetup("-------------------------------------------------");
        new Client.Demo(() => new Client.RemoteRepository(host, port)).RunDemo();

        PrintSetup();
        PrintSetup("-------------------------------------------------");
        PrintSetup("Done.");

        WaitForEnterKey();
    }
}