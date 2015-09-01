// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Common.Model;
    using Remote.Linq;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class Demo
    {
        private readonly string _url;

        public Demo(string url)
        {
            _url = url;
        }

        public void Run()
        {
            var repo = new RemoteRepository(_url);

            Console.WriteLine("\nGET ALL ENTITIES:");
            foreach (var i in repo.Entities)
            {
                Console.WriteLine("  {0}: {1}", i.Id, i);
            }


            Console.WriteLine("\nGET ALL ENTITIES OF TYPE PRODUCT:");
            foreach (var i in repo.Entities.OfType<Product>())
            {
                Console.WriteLine("  {0}: {1}", i.Id, i.Name);
            }


            Console.WriteLine("\nSELECT IDs:");
            var entityIdsQuery =
                from e in repo.Entities
                orderby e.Id ascending
                select new { EntityId = e.Id };
            foreach (var item in entityIdsQuery)
            {
                Console.WriteLine("  {0}", item.EntityId);
            }


            Console.WriteLine("\nCOUNT:");
            var entitiesQuery =
                from p in repo.Entities
                select p;
            Console.WriteLine("  Count = {0}", entitiesQuery.Count());
        }
    }
}
