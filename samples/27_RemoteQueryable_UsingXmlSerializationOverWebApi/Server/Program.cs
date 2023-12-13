// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using static CommonHelper;

    public static class Program
    {
        private static void Main()
        {
            Title("Xml Serialization over Web API [Server]");
            using WebApiServer server = new WebApiServer(8089);
            server.Open();

            PrintSetup("The query service is ready.");
            WaitForEnterKey();
        }
    }
}