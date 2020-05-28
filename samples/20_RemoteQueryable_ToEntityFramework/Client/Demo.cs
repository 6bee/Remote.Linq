// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class AsyncDemo
    {
        private readonly Func<RemoteRepository> _repoProvider;

        public AsyncDemo(Func<RemoteRepository> repoProvider)
        {
            _repoProvider = repoProvider;
        }

        public async Task RunAsync()
        {
            using RemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            var list = await repo.Products
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in list)
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("SELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            var productIds = await productIdsQuery
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (int id in productIdsQuery)
            {
                PrintLine($"  {id}");
            }

            PrintHeader("COUNT:");
            var productsQuery =
                from p in repo.Products
                select p;
            PrintLine($"  Count = {await productsQuery.CountAsync().ConfigureAwait(false)}");

            PrintHeader("INVALID OPERATIONS:");
            try
            {
                _ = await productsQuery
                    .FirstAsync(x => false)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PrintLine($"  {ex.GetType().Name}: {ex.Message}");
            }

            try
            {
                _ = await productsQuery
                    .SingleAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PrintLine($"  {ex.GetType().Name}: {ex.Message}");
            }

            PrintHeader("GET MARKETS WITH PRODUCTS:");
            var marketsWithProducts = await repo.Products
                .SelectMany(x => x.Markets)
                .Include(x => x.Products)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var market in marketsWithProducts)
            {
                PrintLine($"  {market.Name}");

                if (market.Products != null)
                {
                    foreach (var product in market.Products)
                    {
                        PrintLine($"    {product.Name}");
                    }
                }
            }

            PrintHeader("GET ALL PRODUCTS AND THEIR MARKETS:");
            foreach (var item in repo.Products.Include(x => x.Markets))
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");

                foreach (var m in item.Markets)
                {
                    PrintLine($"         {m.Name}");
                }
            }

            PrintHeader("GET ALL PRODUCTS HAVING MARKETS DEFINED:");
            var query = repo.Products
                .Where(p => p.Markets.Any());
            var queryResult = await query
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in queryResult)
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("GET ALL PRODUCTS HAVING MARKETS DEFINED (INCLUDING MARKETS):");
            var queryResultWithMarketData = await query
                .Include(p => p.Markets)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in queryResultWithMarketData)
            {
                var markets = item.Markets.Select(x => x.Name);
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C} | {string.Join("; ", markets)}");
            }
        }
    }
}
