// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
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


            Console.WriteLine("\nSELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
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


            Console.WriteLine("\nINVALID OPERATIONS:");
            try
            {
                var first = productsQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }

            try
            {
                var first = productsQuery.Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }


            Console.WriteLine("\nGET MARKETS WITH PRODUCTS:");
            var marketsQuery = repo.Products.SelectMany(x => x.Markets).Include(x => x.Products);
            var exp = marketsQuery.Expression;
            foreach (var market in marketsQuery)
            {
                Console.WriteLine($"  {market.Name}");

                if (market.Products != null)
                {
                    foreach (var product in market.Products)
                    {
                        Console.WriteLine($"    {product.Name}");
                    }
                }
            }


            Console.WriteLine("\nGET ALL PRODUCTS AND THEIR MARKETS:");
            foreach (var i in repo.Products.Include(x => x.Markets))
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");

                foreach (var m in i.Markets)
                {
                    Console.WriteLine($"         {m.Name}");
                }
            }


            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED:");
            var query = repo.Products.Where(p => p.Markets.Any());
            foreach (var i in query)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
            }


            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED (INCLUDING MARKETS):");
            query = query.Include(p => p.Markets);
            foreach (var i in query)
            {
                var markets = i.Markets.Select(x => x.Name);
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C} | {string.Join("; ", markets)}");
            }
        }
    }
}
