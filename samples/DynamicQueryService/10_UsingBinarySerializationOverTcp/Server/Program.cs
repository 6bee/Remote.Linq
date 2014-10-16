// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var serviceHost = new TcpServer("127.0.0.1", 8899))
            {
				serviceHost.Open();

                Console.WriteLine("The query service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
