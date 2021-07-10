// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using static CommonHelper;

    public static class Program
    {
        private static void Main()
        {
            Title("Simple Remote Query [Client]");
            PrintDemoDescription();
            WaitForEnterKey("Launch the server and then press <ENTER> to run the queries.");

            new Demo().RunDemo();

            PrintSetup("Done");
            WaitForEnterKey("Press <ENTER> to terminate the client.");
        }

        public static void PrintDemoDescription()
        {
            PrintSetup("This sample client retrieves data from the backend by using:");
            PrintSetup(" a) traditional data services (get by id/name)");
            PrintSetup(" b) typed remote linq data services (dynamic filtering, sorting, and paging)");
            PrintSetup(" c) a single generic remote linq data service (dynamic filtering, sorting, and paging)");
            PrintSetup();
        }
    }
}
