// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Client;
    using Common.ServiceContracts;
    using Server;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Custom Result Handler");
            const string url = "net.pipe://localhost/8080/query";

            PrintSetup("Starting WCF service...");
            using var wcfServiceHost = WcfHelper.CreateServiceHost<QueryService>()
                .AddNetNamedPipeEndpoint<IQueryService>(url)
                .OpenService();

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            new Demo(() => new RemoteRepository(url)).Run();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}
