// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using static CommonHelper;

    public static class Program
    {
        private static void Main()
        {
            Title("Entity Framework [Client]");
            WaitForEnterKey("Launch the query service, then press <ENTER> to start the client.");

            new AsyncDemo(() => new RemoteRepository("localhost", 8899)).RunAsync().Wait();

            WaitForEnterKey();
        }
    }
}
