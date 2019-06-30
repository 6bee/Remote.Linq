// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;
    using System.Linq;

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

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (var i in repo.Products)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
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
                Console.WriteLine($"  {i}");
            }


            Console.WriteLine("\nINNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = innerJoinQuery.ToList();
            foreach (var i in innerJoinResult)
            {
                Console.WriteLine($"  {i}");
            }


            Console.WriteLine("\nSELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                select p.Id;
            var productIds = productIdsQuery.ToList();
            foreach (var id in productIdsQuery)
            {
                Console.WriteLine($"  {id}");
            }


            Console.WriteLine("\nCOUNT:");
            var productsQuery =
                from p in repo.Products
                select p;
            Console.WriteLine($"  Count = {productsQuery.Count()}");


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
                    Amount2 = new { Amount = g.Sum(x => x.i.Quantity * x.p.Price) },
                };

            var totalAmountByCategroyResult = totalAmountByCategoryQuery.ToDictionary(x => x.Category);
            foreach (var i in totalAmountByCategroyResult)
            {
                Console.WriteLine($"  {i}");
            }


            Console.WriteLine("\nINVALID OPERATION:");
            try
            {
                var first = totalAmountByCategoryQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }
        }
    }
}
