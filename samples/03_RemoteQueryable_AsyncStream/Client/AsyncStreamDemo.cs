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
            var asyncProductsQueryable = repo.Products.AsAsyncEnumerable();
            await foreach (var item in asyncProductsQueryable.ConfigureAwait(false))
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("CROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select new { Category = "#" + c.Name + sufix("-"), p.Name };
            var asyncCrossJoinQuery = crossJoinQuery.AsAsyncEnumerable();
            await foreach (var item in asyncCrossJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var asyncInnerJoinQuery = innerJoinQuery.AsAsyncEnumerable();
            await foreach (var item in asyncInnerJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("SELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            var asyncProductIdsQuery = productIdsQuery.AsAsyncEnumerable();
            await foreach (int id in asyncProductIdsQuery.ConfigureAwait(false))
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

            var asyncTotalAmountByCategoryQuery = totalAmountByCategoryQuery.AsAsyncEnumerable();
            await foreach (var item in asyncTotalAmountByCategoryQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }
        }
    }
}
