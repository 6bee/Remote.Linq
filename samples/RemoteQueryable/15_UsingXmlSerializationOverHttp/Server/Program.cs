// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            using (HttpServer serviceHost = new HttpServer(8089))
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
