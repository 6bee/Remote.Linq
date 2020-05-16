// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            const string host = "localhost";
            const int port = 8089;

            Console.WriteLine("Starting Web API service...");

            using (Server.WebApiServer webApiServer = new Server.WebApiServer(port))
            {
                webApiServer.Open();

                Console.WriteLine("Started query service.");

                Console.WriteLine("Staring client demo...");
                Console.WriteLine("-------------------------------------------------");

                new Client.Demo(host, port).Run();

                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------");

                Console.WriteLine("Done.");
            }

            Console.WriteLine("Terminated Web API service. Hit enter to exit.");
            Console.ReadLine();
        }
    }
}
