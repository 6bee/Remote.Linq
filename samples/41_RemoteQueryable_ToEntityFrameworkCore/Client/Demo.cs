// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq.Async;
    using Remote.Linq.Include;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;
    using DbFunctionsExtensions = Microsoft.EntityFrameworkCore.DbFunctionsExtensions;

    public class Demo : IAsyncDemo
    {
        private readonly Func<RemoteRepository> _repoProvider;

        public Demo(Func<RemoteRepository> repoProvider)
            => _repoProvider = repoProvider;

        public async Task RunAsync()
        {
            using RemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            var list = await repo.Products
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in list)
            {
                PrintLine($"  P#{item.Id}| {item.Name} | {item.Price:C}");
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
                PrintLine($"  P#{id}");
            }

            PrintHeader("COUNT:");
            var asyncProductCount = repo.Products.CountAsync().ConfigureAwait(false);
            PrintLine($"  Count = {await asyncProductCount}");

            PrintHeader("INVALID OPERATIONS:");
            try
            {
                _ = await repo.Products
                    .FirstAsync(x => false)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PrintLine($"  {ex.GetType().Name}: {ex.Message}");
            }

            try
            {
                _ = await repo.Products
                    .SingleAsync()
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PrintLine($"  {ex.GetType().Name}: {ex.Message}");
            }

            PrintHeader("GET MARKETS WITH APPLES:");
            var marketsWithProducts = repo.Markets
                .Where(x => x.Products.Any(r => DbFunctionsExtensions.Like(null, r.Product.Name, "app%")))
                .Include(x => x.Products.Where(xp => DbFunctionsExtensions.Like(null, xp.Product.Name, "app%"))).ThenInclude(x => x.Product)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var market in await marketsWithProducts)
            {
                PrintLine($"  M#{market.Id}| {market.Name}");

                foreach (var product in market.Products)
                {
                    PrintLine($"        P#{product.Product.Id}| {product.Product.Name}");
                }
            }

            PrintHeader("GET ALL PRODUCTS AND RELATED PRODUCTS OF CORRESPONDING MARKETS:");
            var productMarkets = await repo.Products
                .Include(x => x.Markets).ThenInclude(x => x.Market).ThenInclude(x => x.Products)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in productMarkets)
            {
                PrintLine($"  P#{item.Id}| {item.Name} | {item.Price:C}");

                var relatedProducts = item.Markets
                    .Select(m => m.Market)
                    .SelectMany(m => m.Products
                        .Select(mp => new { mp.MarketId, mp.Product })
                        .Where(x => x.Product != item));
                foreach (var p in relatedProducts)
                {
                    PrintLine($"         M#{p.MarketId}| P#{p.Product.Id}| {p.Product.Name}");
                }
            }

            PrintHeader("GET ALL PRODUCTS HAVING MARKETS DEFINED (USING INCLUDE):");
            var query = repo.Products
                .Where(p => p.Markets.Any());
            var productIncludingMarketData = query
                .Include(x => x.Markets).ThenInclude(x => x.Market)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in await productIncludingMarketData)
            {
                var markets = item.Markets.Select(x => x.Market.Name);
                PrintLine($"  P#{item.Id}| {item.Name} | {string.Join("; ", markets)}");
            }

            PrintHeader("GET ALL PRODUCTS HAVING MARKETS DEFINED (USING PROJECTION):");
            var productSelectingMarketData = query
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    Markets = x.Markets.Select(m => m.Market.Name),
                })
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var item in await productSelectingMarketData)
            {
                PrintLine($"  P#{item.Id}| {item.Name} | {string.Join("; ", item.Markets)}");
            }

            PrintHeader("GROUP BY:");
            var grouped =
                from p in repo.Products
                group p.Name by p.ProductCategory.Name into g
                select new
                {
                    Category = g.Key,
                    NumberOfProducts = g.Count(),
                };
            foreach (var g in await grouped.ToListAsync().ConfigureAwait(false))
            {
                PrintLine($"  {g.NumberOfProducts} {g.Category.ToLower()}");
            }
        }
    }
}
