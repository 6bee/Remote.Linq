// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class AsyncStreamDemo : IAsyncDemo
    {
        private readonly Func<IAsyncRemoteRepository> _repoProvider;

        public AsyncStreamDemo(Func<IAsyncRemoteRepository> repoProvider)
            => _repoProvider = repoProvider;

        public async Task RunAsync()
        {
            await using IAsyncRemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS [ASYNC STREAM]:");
            await foreach (var item in repo.Products.ConfigureAwait(false))
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("CROSS JOIN [ASYNC STREAM]:");
            var vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
            var crossJoinQuery =
                from c in repo.ProductCategories
                from p in repo.Products
                let @particle = vowels.Contains(p.Name.Substring(0, 1).ToLower().Single()) ? "An" : "A"
                let @subject = p.Name.ToLower()
                let @verb = $"is{(c.Id == p.ProductCategoryId ? null : " not")} a"
                let @object = c.Name.ToLower().TrimEnd('s')
                orderby @subject, @object
                select $"{@particle} {@subject} {@verb} {@object}";
            await foreach (var item in crossJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("CROSS JOIN [ASYNC QUERY]:");
            foreach (var item in await crossJoinQuery.ToListAsync().ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN [ASYNC STREAM]:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { Product = new { Description = string.Concat(c.Name, "-", p.Name), Price = $"{p.Price:C}" } };
            await foreach (var item in innerJoinQuery.ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("SELECT IDs [ASYNC STREAM]:");
            var productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            await foreach (int id in productIdsQuery.ConfigureAwait(false))
            {
                PrintLine($"  {id}");
            }

            PrintHeader("GROUP BY [ASYNC STREAM]:");
            var grouped =
                from p in repo.Products
                join c in repo.ProductCategories on p.ProductCategoryId equals c.Id
                orderby c.Name descending, p.Name ascending
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