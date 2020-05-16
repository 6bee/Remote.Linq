// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContracts;
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    internal class Program
    {
        private static void Main()
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(QueryService)))
            {
                serviceHost.Description.Behaviors.OfType<ServiceDebugBehavior>().Single().IncludeExceptionDetailInFaults = true;
                serviceHost.AddServiceEndpoint(typeof(IQueryService), new NetNamedPipeBinding(), "net.pipe://localhost/8080/query");

                serviceHost.Open();

                Console.WriteLine("The query service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();
            }
        }
    }
}
