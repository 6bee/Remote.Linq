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
        private readonly Func<IAsyncRemoteRepository> _repoProvider;

        public AsyncStreamDemo(Func<AsyncRemoteRepository> repoProvider)
        {
            _repoProvider = repoProvider;
        }

        public async ValueTask RunAsync()
        {
            await using IAsyncRemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            await foreach (var item in repo.Products.ConfigureAwait(false))
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("CROSS JOIN:");
            Func<object, string> sufix = (x) => x + "ending";
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                select $"A {p.Name.ToLower()} is {(c.Id == p.ProductCategoryId ? null : "not ")}a {c.Name.ToLower().TrimEnd('s')}";
            await foreach (var item in crossJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { Category = c.Name, Price = $"{p.Price:C}", X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            await foreach (var item in innerJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("SELECT IDs:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            await foreach (int id in productIdsQuery.ConfigureAwait(false))
            {
                PrintLine($"  {id}");
            }

            PrintHeader("SELECT GOUPING:");
            var grouped =
                from p in repo.Products
                join c in repo.ProductCategories on p.ProductCategoryId equals c.Id
                group p.Name by c.Name into g
                select g;
            await foreach (var g in grouped.ConfigureAwait(false))
            {
                PrintLine($"  {g.Key}");
                await foreach (var p in g)
                {
                    PrintLine($"      {p}");
                }
            }
        }
    }
}
