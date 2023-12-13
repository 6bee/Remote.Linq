// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContract;
    using static CommonHelper;

    public class Program
    {
        public static void Main()
        {
            Title("Simple Remote Query [Server]");

            using var dataServiceHost = WcfHelper.CreateServiceHost<TraditionalDataService>()
                .AddNetTcpEndpoint<ITraditionalDataService>("net.tcp://localhost:8080/traditionaldataservice")
                .OpenService();

            using var remoteLinqDataServiceHost = WcfHelper.CreateServiceHost<RemoteLinqDataService>()
                .AddNetTcpEndpoint<IRemoteLinqDataService>("net.tcp://localhost:8080/remotelinqdataservice")
                .OpenService();

            PrintSetup("The server is ready.");
            WaitForEnterKey();
        }
    }
}