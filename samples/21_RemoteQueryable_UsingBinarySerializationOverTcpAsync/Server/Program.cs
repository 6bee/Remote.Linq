﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using static CommonHelper;

internal static class Program
{
    private static void Main()
    {
        Title("Binary Serialization over TCP/IP async [Server]");
        using var serviceHost = new TcpServer(8899);
        serviceHost.RunAsyncQueryService(new QueryService().ExecuteQueryAsync);

        PrintServerReady();
        WaitForEnterKey();
    }
}