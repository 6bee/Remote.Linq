// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class AsyncStreamDemo
    {
        private readonly Func<IRemoteRepository> _repoProvider;

        public AsyncStreamDemo(Func<RemoteAsyncStreamRepository> repoProvider)
        {
            _repoProvider = repoProvider;
        }

        public async Task RunAsync()
        {
            using IRemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            var allProducts = repo.Products
                .AsAsyncEnumerable()
                .ConfigureAwait(false);
            await foreach (var item in allProducts)
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("CROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = "#" + c.Name + sufix("-"), p.Name };
            var crossJoinResult = crossJoinQuery
                .AsAsyncEnumerable()
                .ConfigureAwait(false);
            await foreach (var item in crossJoinResult)
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = innerJoinQuery
                .AsAsyncEnumerable()
                .ConfigureAwait(false);
            await foreach (var item in innerJoinResult)
            {
                PrintLine($"  {item}");
            }

            PrintHeader("SELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            var productIds = productIdsQuery
                .AsAsyncEnumerable()
                .ConfigureAwait(false);
            await foreach (int id in productIds)
            {
                PrintLine($"  {id}");
            }

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

            var totalAmountByCategroyResult = totalAmountByCategoryQuery
                .AsAsyncEnumerable()
                .ConfigureAwait(false);
            await foreach (var item in totalAmountByCategroyResult)
            {
                PrintLine($"  {item}");
            }
        }
    }
}
