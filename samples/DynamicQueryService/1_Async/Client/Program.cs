// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wait for query service to indicate that it's started, ");
            Console.WriteLine("then press <ENTER> to start the client.");
            Console.ReadLine();

            new Demo("net.pipe://localhost/8080/query").RunAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
