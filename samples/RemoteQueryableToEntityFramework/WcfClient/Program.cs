// Copyright (c) Christof Senn. All rights reserved. 

using System;
using System.Diagnostics;
using System.Linq;

namespace WcfClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new RemoteRepository("http://localhost:50105/QueryService.svc");

            Console.WriteLine("\n\nCROSS JOIN:\n-----------------------------------------");
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


            Console.WriteLine("\n\nINNER JOIN:\n-----------------------------------------");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { X = new { Y = new { c.Name } }, P = new { p.Price }, Z = new { Q = new { K = string.Concat(c.Name, "-", p.Name) } } };
            var innerJoinResult = innerJoinQuery.ToList();
            foreach (var i in innerJoinResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\n\nTOTAL AMOUNT BY CATEGORY:\n-----------------------------------------");
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
                    Amount2 = new { Amount = g.Sum(x => x.i.Quantity * x.p.Price) },
                };

            var totalAmountByCategroyResult = totalAmountByCategoryQuery.ToDictionary(x => x.Category);
            foreach (var i in totalAmountByCategroyResult)
            {
                Console.WriteLine("  {0}", i);
            }


            Console.WriteLine("\n\nINVALID OPERATION:\n-----------------------------------------");
            try
            {
                var first = totalAmountByCategoryQuery.Where(x => false).First();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("  {0}", ex.Message);
                Debug.WriteLine("  {0}", ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.WriteLine("Press <ENTER> to terminate the client.");
            Console.ReadLine();
        }
    }
}
