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
            RemoteRepository repo = new RemoteRepository(_url);

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (Common.Model.Product i in repo.Products)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
            }

            Console.WriteLine("\nINNER JOIN FOR FILTERING:");
            Func<object, string> sufix = (x) => x + "ending";
            IQueryable<Common.Model.Product> crossJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                where c.Name == "Fruits"
                select p;
            System.Collections.Generic.List<Common.Model.Product> crossJoinResult = crossJoinQuery.ToList();
            foreach (Common.Model.Product i in crossJoinResult)
            {
                Console.WriteLine($"  {i.Id} - {i.Name}");
            }

            Console.WriteLine("\nINNER JOIN AND PROJECTION TO STRING:");
            IQueryable<string> innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select string.Concat(c.Name, "-", p.Name);
            System.Collections.Generic.List<string> innerJoinResult = innerJoinQuery.ToList();
            foreach (string i in innerJoinResult)
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

            Console.WriteLine("\nMAX TOTAL AMOUNT BY CATEGORY:");
            IQueryable<decimal> totalAmountByCategoryQuery =
                from c in repo.ProductCategories
                join p in repo.Products
                    on c.Id equals p.ProductCategoryId
                join i in repo.OrderItems
                    on p.Id equals i.ProductId
                group new { c, p, i } by c.Name into g
                select g.Sum(x => x.i.Quantity * x.p.Price);

            Console.WriteLine($"  {totalAmountByCategoryQuery.Max()}");

            Console.WriteLine("\nINVALID OPERATION:");
            try
            {
                decimal first = totalAmountByCategoryQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }
        }
    }
}
