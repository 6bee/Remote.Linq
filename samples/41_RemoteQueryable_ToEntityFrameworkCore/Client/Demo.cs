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

            PrintHeader("GET MARKETS WITH PRODUCTS:");
            var marketsWithProducts = repo.Markets
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Where(x => x.Products.Any())
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
        }
    }
}
