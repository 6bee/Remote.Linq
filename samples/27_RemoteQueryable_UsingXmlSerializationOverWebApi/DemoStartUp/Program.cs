// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using static CommonHelper;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            ParseContextArgs(args);
            Title("Xml Serialization over Web API");
            const string host = "localhost";
            const int port = 8089;

            PrintSetup("Starting Web API service...");
            using var webServer = new Server.WebApiServer(port);
            webServer.Open();

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            new Client.AsyncDemo(() => new Client.RemoteRepository(host, port)).RunAsync().Wait();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}
