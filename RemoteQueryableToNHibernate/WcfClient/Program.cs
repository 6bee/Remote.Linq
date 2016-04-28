// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace WcfClient
{
    using Remote.Linq;
    using Remote.Linq;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Run();

            Console.WriteLine();
            Console.WriteLine("Press <ENTER> to terminate.");
            Console.WriteLine();
            Console.ReadLine();
        }

        private static void Run()
        {            
            var repo = new RemoteRepository("http://localhost:50106/QueryService.svc");

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (var i in repo.Products.Include(x => x.Name))
            {
                Console.WriteLine("  {0} | {1} | {2:C}", i.Id, i.Name, i.Price);
            }


            Console.WriteLine("\nCROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = "#" + c.Name + sufix("-"), p.Name };
            var crossJoinResult = crossJoinQuery.ToList();
            foreach (var i in crossJoinResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\nINNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = innerJoinQuery.ToList();
            foreach (var i in innerJoinResult)
            {
                Console.WriteLine("  {0}", i);
            }


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

            var totalAmountByCategroyResult = totalAmountByCategoryQuery.ToDictionary(x => x.Category);
            foreach (var i in totalAmountByCategroyResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\nINVALID OPERATION:");
            try
            {
                var first = totalAmountByCategoryQuery.First(x => false);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("  {0}", ex.Message);
                Debug.WriteLine("  {0}", ex.Message);
            }
        }
    }
}
