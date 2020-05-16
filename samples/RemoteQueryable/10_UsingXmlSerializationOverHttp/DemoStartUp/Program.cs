// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using Server;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            const string host = "localhost";
            const int port = 8089;

            Console.WriteLine("Starting HTTP service...");

            using (var httpServiceHost = new HttpServer(port))
            {
                httpServiceHost.Open();

                Console.WriteLine("Started query service.");

                Console.WriteLine("Staring client demo...");
                Console.WriteLine("-------------------------------------------------");


                var url = $"http://{host}:{port}/queryservice/";
                new Client.Demo(url).RunAsync().Wait();


                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------");

                Console.WriteLine("Done.");
            }

            Console.WriteLine("Terminated HTTP service. Hit enter to exit.");
            Console.ReadLine();
        }
    }
}
