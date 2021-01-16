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
        private readonly Func<IRemoteRepository> _repoProvider;

        public AsyncDemo(Func<IRemoteRepository> repoProvider)
        {
            _repoProvider = repoProvider;
        }

        public async Task RunAsync()
        {
            using IRemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            foreach (var item in await repo.Products.ToArrayAsync().ConfigureAwait(false))
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("CROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = "#" + c.Name + sufix("-"), p.Name };
            var crossJoinResult = await crossJoinQuery.ToListAsync().ConfigureAwait(false);
            foreach (var item in crossJoinResult)
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = await innerJoinQuery.ToListAsync().ConfigureAwait(false);
            foreach (var item in innerJoinResult)
            {
                PrintLine($"  {item}");
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

            PrintHeader("TOTAL AMOUNT BY CATEGORY:");
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

            var totalAmountByCategroyResult = await totalAmountByCategoryQuery
                .ToDictionaryAsync(x => x.Category)
                .ConfigureAwait(false);
            foreach (var item in totalAmountByCategroyResult)
            {
                PrintLine($"  {item}");
            }

            PrintHeader("EXPECTED INVALID OPERATION:");
            try
            {
                _ = await totalAmountByCategoryQuery
                    .Select(x => x.Category)
                    .FirstAsync(x => false)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                PrintLine($"  {ex.GetType().Name}: {ex.Message}");
            }

            PrintHeader("GROUP BY:");
            var grouped =
                from p in repo.Products
                join c in repo.ProductCategories on p.ProductCategoryId equals c.Id
                orderby c.Name descending, p.Name ascending
                group p.Name by c.Name into g
                select g;
            foreach (var g in await grouped.ToListAsync().ConfigureAwait(false))
            {
                PrintLine($"  {g.Key}");
                foreach (var p in g)
                {
                    PrintLine($"      {p}");
                }
            }
        }
    }
}
