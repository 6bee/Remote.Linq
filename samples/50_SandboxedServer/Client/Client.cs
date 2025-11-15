// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common;
using System.IO;
using System.Security;
using static CommonHelper;

public class Client : IDemo
{
    private readonly RemoteRepository _repo;

    public Client(string ip, int port)
        => _repo = new RemoteRepository(ip, port);

    public void Run()
    {
        IQueryable<string> query = _repo.Queryable
            .SelectMany(x => Directory.GetLogicalDrives())
            .SelectMany(x => Directory.GetDirectories(x));

        try
        {
            using var color = TextColor(ConsoleColor.Red);
            query.ForEach(PrintLine);
        }
        catch (Exception ex)
        {
            while (ex.InnerException is SecurityException inner)
            {
                ex = inner;
            }

            using var color = TextColor(ex is SecurityException ? ConsoleColor.Green : ConsoleColor.Red);
            PrintLine($"{ex.GetType()}: {ex.Message}");
        }
    }
}