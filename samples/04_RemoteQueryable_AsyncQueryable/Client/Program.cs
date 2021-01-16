// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using static CommonHelper;

    internal static class Program
    {
        private static void Main()
        {
            Title("Ix.NET [Client]");
            PrintNote("This demo uses a random delay on server side to simulate data item retrieval/generation.");
            WaitForEnterKey("Launch the query service, then press <ENTER> to start the client.");

            new AsyncStreamDemo(() => new AsyncRemoteRepository("localhost", 8899)).RunAsync().AsTask().Wait();

            WaitForEnterKey();
        }
    }
}
