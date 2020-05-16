// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using System;

    public class Program
    {
        private static void Main(string[] args)
        {
            using (WebApiServer server = new WebApiServer(8089))
            {
                server.Open();

                Console.WriteLine("The query service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
