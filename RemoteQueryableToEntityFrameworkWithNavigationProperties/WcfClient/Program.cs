// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace WcfClient
{
    using Remote.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate.");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            var repo = new RemoteRepository("http://localhost:50105/QueryService.svc");


            Console.WriteLine("\nGET MARKETS WITH PRODUCTS:");
            var marketsQuery = repo.Products.SelectMany(x => x.Markets).Include(x => x.Products);
            var exp = marketsQuery.Expression;
            foreach (var market in marketsQuery)
            {
                Console.WriteLine("  {0}", market.Name);

                if (market.Products != null)
                {
                    foreach (var product in market.Products)
                    {
                        Console.WriteLine("    {0}", product.Name);
                    }
                }
            }


            Console.WriteLine("\nGET ALL PRODUCTS AND THEIR MARKETS:");
            foreach (var i in repo.Products.Include(x => x.Markets))
            {
                Console.WriteLine("  {0} | {1} | {2:C}", i.Id, i.Name, i.Price);

                foreach (var m in i.Markets)
                {
                    Console.WriteLine("         {0}", m.Name);
                }
            }


            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED:");
            var query = repo.Products.Where(p => p.Markets.Any());
            foreach (var i in query)
            {
                Console.WriteLine("  {0} | {1} | {2:C}", i.Id, i.Name, i.Price);
            }


            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED (INCLUDING MARKETS):");
            query = query.Include(p => p.Markets);
            foreach (var i in query)
            {
                var markets = i.Markets.Select(x => x.Name);
                Console.WriteLine("  {0} | {1} | {2:C} | {3}", i.Id, i.Name, i.Price, string.Join("; ", markets));
            }
        }
    }
}
