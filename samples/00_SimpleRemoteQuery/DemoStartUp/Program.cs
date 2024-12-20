// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp;

using static CommonHelper;

internal static class Program
{
    private static void Main(string[] args)
    {
        ParseContextArgs(args);
        Title("Simple Remote Query [Server]");
        Client.Program.PrintDemoDescription();

        PrintSetup("Starting WCF service...");
        using var dataServiceHost = WcfHelper.CreateServiceHost<Server.TraditionalDataService>()
            .AddNetTcpEndpoint<Common.ServiceContract.ITraditionalDataService>("net.tcp://localhost:8080/traditionaldataservice")
            .OpenService();

        using var remoteLinqDataServiceHost = WcfHelper.CreateServiceHost<Server.RemoteLinqDataService>()
            .AddNetTcpEndpoint<Common.ServiceContract.IRemoteLinqDataService>("net.tcp://localhost:8080/remotelinqdataservice")
            .OpenService();

        PrintSetup("Starting client demo...");
        new Client.Demo().RunDemo();

        PrintSetup();
        PrintSetup("Done.");
        WaitForEnterKey();
    }
}