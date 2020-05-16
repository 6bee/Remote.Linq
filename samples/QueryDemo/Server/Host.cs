// Copyright (c) Christof Senn. All rights reserved. 

using System;
using System.ServiceModel;

namespace Server
{
    public class Host
    {
        public static void Main()
        {
            ServiceHost dataServiceHost=null;
            ServiceHost remoteLinqDataServiceHost = null;
            try
            {
                dataServiceHost = new ServiceHost(typeof(DataService));
                dataServiceHost.Open();

                remoteLinqDataServiceHost = new ServiceHost(typeof(RemoteLinqDataService));
                remoteLinqDataServiceHost.Open();
                
                Console.WriteLine("The server is ready.");
                Console.WriteLine("Press <ENTER> to terminate services.");
                Console.WriteLine();
                Console.ReadLine();
            }
            finally
            {
                dataServiceHost?.Close();
                remoteLinqDataServiceHost?.Close();
            }
        }
    }
}
