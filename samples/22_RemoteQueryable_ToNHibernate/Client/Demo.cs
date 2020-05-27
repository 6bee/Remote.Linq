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
            using RemoteRepository repo = new RemoteRepository(_url);

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (Common.Model.Product i in repo.Products)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
            }

            Console.WriteLine("\nCROSS JOIN:");
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = c.Name, Product = p.Name };
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
            IQueryable<int> productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            System.Collections.Generic.List<int> productIds = productIdsQuery.ToList();
            foreach (int id in productIdsQuery)
            {
                Console.WriteLine($"  {id}");
            }

            Console.WriteLine("\nCOUNT:");
            IQueryable<Common.Model.Product> productsQuery =
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
                var first = innerJoinQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }
        }
    }
}
