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
            const int port = 8899;

            Console.WriteLine("Starting TCP service...");

            using (var tcpServiceHost = new TcpServer(port))
            {
                tcpServiceHost.Open();

                Console.WriteLine("Started query service.");

                Console.WriteLine("Staring client demo...");
                Console.WriteLine("-------------------------------------------------");


                new Client.Demo(host, port).Run();


                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------");

                Console.WriteLine("Done.");
            }

            Console.WriteLine("Terminated TCP service. Hit enter to exit.");
            Console.ReadLine();
        }
    }
}
