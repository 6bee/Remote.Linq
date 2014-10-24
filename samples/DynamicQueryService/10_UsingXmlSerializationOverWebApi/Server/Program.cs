// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // load common assembly
            var asm = typeof(Common.Model.ProductCategory).Assembly;

            var config = new HttpSelfHostConfiguration("http://localhost:8898");

            config.Routes.MapHttpRoute("API Default", "api/{controller}");
            config.MaxReceivedMessageSize = 10 * 1024 * 1024;

            using (var server = new HttpSelfHostServer(config))
            {
                server.OpenAsync().Wait();

                Console.WriteLine("The query service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
