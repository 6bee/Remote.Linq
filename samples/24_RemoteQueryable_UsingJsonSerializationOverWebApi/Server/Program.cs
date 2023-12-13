// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using static CommonHelper;

    public static class Program
    {
        private static void Main()
        {
            Title("JSON Serialization over Web API async [Server]");
            using var webServer = new WebApiServer(8089);
            webServer.Open();

            PrintServerReady();
            WaitForEnterKey();
        }
    }
}