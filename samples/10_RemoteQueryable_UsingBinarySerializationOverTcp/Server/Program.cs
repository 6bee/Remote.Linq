// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using System;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            using (TcpServer tcpServiceHost = new TcpServer("127.0.0.1", 8899))
            {
                tcpServiceHost.Open();

                Console.WriteLine("The query service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
