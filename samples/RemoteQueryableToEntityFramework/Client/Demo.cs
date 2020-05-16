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
            RemoteRepository repo = new RemoteRepository(_url);

            Console.WriteLine("\nGET ALL PRODUCTS:");
            foreach (Common.Model.Product i in repo.Products)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
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

            Console.WriteLine("\nINVALID OPERATIONS:");
            try
            {
                Common.Model.Product first = productsQuery.First(x => false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }

            try
            {
                Common.Model.Product first = productsQuery.Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  {ex.Message}");
            }

            Console.WriteLine("\nGET MARKETS WITH PRODUCTS:");
            IQueryable<Common.Model.Market> marketsQuery = repo.Products.SelectMany(x => x.Markets).Include(x => x.Products);
            System.Linq.Expressions.Expression exp = marketsQuery.Expression;
            foreach (Common.Model.Market market in marketsQuery)
            {
                Console.WriteLine($"  {market.Name}");

                if (market.Products != null)
                {
                    foreach (Common.Model.Product product in market.Products)
                    {
                        Console.WriteLine($"    {product.Name}");
                    }
                }
            }

            Console.WriteLine("\nGET ALL PRODUCTS AND THEIR MARKETS:");
            foreach (Common.Model.Product i in repo.Products.Include(x => x.Markets))
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");

                foreach (Common.Model.Market m in i.Markets)
                {
                    Console.WriteLine($"         {m.Name}");
                }
            }

            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED:");
            IQueryable<Common.Model.Product> query = repo.Products.Where(p => p.Markets.Any());
            foreach (Common.Model.Product i in query)
            {
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C}");
            }

            Console.WriteLine("\nGET ALL PRODUCTS HAVING MARKETS DEFINED (INCLUDING MARKETS):");
            query = query.Include(p => p.Markets);
            foreach (Common.Model.Product i in query)
            {
                System.Collections.Generic.IEnumerable<string> markets = i.Markets.Select(x => x.Name);
                Console.WriteLine($"  {i.Id} | {i.Name} | {i.Price:C} | {string.Join("; ", markets)}");
            }
        }
    }
}
