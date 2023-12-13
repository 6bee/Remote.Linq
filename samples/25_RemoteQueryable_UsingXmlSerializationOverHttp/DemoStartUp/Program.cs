// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Server;
    using static CommonHelper;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            ParseContextArgs(args);
            Title("Xml Serialization over Http");
            const string host = "localhost";
            const int port = 8089;

            PrintSetup("Starting HTTP service...");
            using var httpServer = new HttpServer(port);
            httpServer.RunQueryService(new QueryService().ExecuteQueryAsync);

            PrintSetup("Staring client demo...");
            PrintSetup("-------------------------------------------------");
            string url = $"http://{host}:{port}/queryservice/";
            new Client.AsyncDemo(() => new Client.RemoteRepository(url)).RunAsyncDemo();

            PrintSetup();
            PrintSetup("-------------------------------------------------");
            PrintSetup("Done.");
            WaitForEnterKey();
        }
    }
}