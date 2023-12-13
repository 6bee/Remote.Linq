// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Inheritance [Server]");
            using var serviceHost = WcfHelper.CreateServiceHost<QueryService>()
                .AddNetNamedPipeEndpoint<Common.ServiceContracts.IQueryService>("net.pipe://localhost/8080/query")
                .OpenService();

            PrintServerReady();
            WaitForEnterKey();
        }
    }
}