﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("Custom Type Resolver");
        const string url = "net.pipe://localhost/8080/query";

        PrintSetup("Starting WCF service...");
        using var serviceHost = WcfHelper.CreateServiceHost<Server.QueryService>()
            .AddNetNamedPipeEndpoint<Common.ServiceContracts.IQueryService>(url)
            .OpenService();

        PrintSetup("Starting client demo...");
        PrintSetup("-------------------------------------------------");
        new Client.Demo(() => new Client.RemoteRepository(url)).RunDemo();

        PrintSetup();
        PrintSetup("-------------------------------------------------");
        PrintSetup("Done.");
        WaitForEnterKey();
    }
}