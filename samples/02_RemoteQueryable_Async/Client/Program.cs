﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using static CommonHelper;

internal static class Program
{
    private static void Main()
    {
        Title("Async [Client]");
        WaitForEnterKey("Launch the query service, then press <ENTER> to start the client.");

        new AsyncDemo(() => new RemoteRepository("net.pipe://localhost/8080/query")).RunAsyncDemo();

        WaitForEnterKey();
    }
}