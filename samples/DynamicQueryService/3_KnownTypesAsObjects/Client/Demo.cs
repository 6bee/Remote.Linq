// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;
    using System.Diagnostics;
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
                Console.WriteLine("  {0} | {1} | {2:C}", i.Id, i.Name, i.Price);
            }

            Console.WriteLine("\nINNER JOIN FOR FILTERING:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                where c.Name == "Fruits"
                select p;
            var crossJoinResult = crossJoinQuery.ToList();
            foreach (var i in crossJoinResult)
            {
                Console.WriteLine("  {0} - {1}", i.Id, i.Name);
            }


            Console.WriteLine("\nINNER JOIN AND PROJECTION TO STRING:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select string.Concat(c.Name, "-", p.Name);
            var innerJoinResult = innerJoinQuery.ToList();
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


            Console.WriteLine("\nMAX TOTAL AMOUNT BY CATEGORY:");
            var totalAmountByCategoryQuery =
                from c in repo.ProductCategories
                join p in repo.Products
                    on c.Id equals p.ProductCategoryId
                join i in repo.OrderItems
                    on p.Id equals i.ProductId
                group new { c, p, i } by c.Name into g
                select g.Sum(x => x.i.Quantity * x.p.Price);

            Console.WriteLine("  {0}", totalAmountByCategoryQuery.Max());


            Console.WriteLine("\nINVALID OPERATION:");
            try
            {
                var first = totalAmountByCategoryQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("  {0}", ex.Message);
                Debug.WriteLine("  {0}", ex.Message);
            }
        }
    }
}
