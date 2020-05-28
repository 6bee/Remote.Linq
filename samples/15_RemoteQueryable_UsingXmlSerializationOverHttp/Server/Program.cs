// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Xml Serialization over Http [Server]");
            using var serviceHost = new HttpServer(8089);
            serviceHost.RunQueryService(new QueryService().ExecuteQueryAsync);

            PrintServerReady();
            WaitForEnterKey();
        }
    }
}
