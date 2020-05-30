// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class Demo
    {
        private readonly Func<RemoteRepository> _repoProvider;

        public Demo(Func<RemoteRepository> repoProvider)
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

            var productsQuery =
                from p in repo.Products
                select p;

            PrintHeader("COUNT:");
            var asyncProductCount = productsQuery.CountAsync().ConfigureAwait(false);
            PrintLine($"  Count = {await asyncProductCount}");

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
            var marketsWithProducts = await repo.Markets
                .Include(x => x.Products)
                .Where(x => x.Products.Any())
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
            var productsAndMarkets = await repo.Products
                .Include(x => x.Markets)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in productsAndMarkets)
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
