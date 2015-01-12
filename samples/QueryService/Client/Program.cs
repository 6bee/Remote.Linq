// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wait for query service to indicate that it's started, ");
            Console.WriteLine("then press <ENTER> to start the client.");
            Console.ReadLine();

            RunAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate.");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static async Task RunAsync()
        {
            var repo = new RemoteRepository("net.pipe://localhost/8080/query");

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (var i in repo.Products)
            {
                Console.WriteLine("  {0} | {1} | {2:C}", i.Id, i.Name, i.Price);
            }

            Console.WriteLine("\nCROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = "#" + c.Name + sufix("-"), p.Name };
            var crossJoinResult = await crossJoinQuery.ToListAsync();
            foreach (var i in crossJoinResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\nINNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = await innerJoinQuery.ToListAsync();
            foreach (var i in innerJoinResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\nSELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                select p.Id;
            var productIds = productIdsQuery.ToList();
            foreach (var id in productIdsQuery)
            {
                Console.WriteLine("  {0}", id);
            }

            Console.WriteLine("\nCOUNT:");
            var productsQuery =
                from p in repo.Products
                select p;
            Console.WriteLine("  Count = {0}", productsQuery.Count());


            Console.WriteLine("\nTOTAL AMOUNT BY CATEGORY:");
            var totalAmountByCategoryQuery =
                from c in repo.ProductCategories
                join p in repo.Products
                    on c.Id equals p.ProductCategoryId
                join i in repo.OrderItems
                    on p.Id equals i.ProductId
                group new { c, p, i } by c.Name into g
                select new
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.i.Quantity * x.p.Price),
                };

            var totalAmountByCategroyResult = await totalAmountByCategoryQuery.ToDictionaryAsync(x => x.Category);
            foreach (var i in totalAmountByCategroyResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\nINVALID OPERATION:");
            try
            {
                var first = await totalAmountByCategoryQuery.FirstAsync(x => false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("  {0}", ex.Message);
                Debug.WriteLine("  {0}", ex.Message);
            }
        }
    }
}
