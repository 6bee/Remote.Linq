// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace DemoStartUp
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string url = "net.pipe://localhost/8080/query";

            Console.WriteLine("Starting WCF service...");

            using (ServiceHost wcfServiceHost = new ServiceHost(typeof(Server.QueryService)))
            {
                wcfServiceHost.Description.Behaviors.OfType<ServiceDebugBehavior>().Single().IncludeExceptionDetailInFaults = true;
                wcfServiceHost.AddServiceEndpoint(typeof(Common.ServiceContracts.IQueryService), new NetNamedPipeBinding(), url);

                wcfServiceHost.Open();

                Console.WriteLine("Started query service.");

                Console.WriteLine("Staring client demo...");
                Console.WriteLine("-------------------------------------------------");

                new Client.Demo(url).Run();

                Console.WriteLine();
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine("Done.");
            }

            Console.WriteLine("Terminated WCF service. Hit enter to exit");
            Console.ReadLine();
        }
    }
}
