// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security;

    public class Client
    {
        private readonly RemoteRepository _repo;

        public Client(string ip, int port)
        {
            _repo = new RemoteRepository(ip, port);
        }

        public void Run()
        {
            IQueryable<string> query = _repo.Queryable
                .SelectMany(x => Directory.GetLogicalDrives())
                .SelectMany(x => Directory.GetDirectories(x));

            try
            {
                foreach (string result in query)
                {
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException is SecurityException inner)
                {
                    ex = inner;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex.GetType()}: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
