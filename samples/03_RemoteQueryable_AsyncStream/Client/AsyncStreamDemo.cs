// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
    using Remote.Linq.Async;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class AsyncStreamDemo : IAsyncDemo
    {
        private readonly Func<IRemoteRepository> _repoProvider;

        public AsyncStreamDemo(Func<AsyncRemoteStreamRepository> repoProvider)
            => _repoProvider = repoProvider;

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
            await foreach (var item in crossJoinQuery.AsAsyncEnumerable().ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { Product = new { Description = string.Concat(c.Name, "-", p.Name), Price = $"{p.Price:C}" } };
            await foreach (var item in innerJoinQuery.AsAsyncEnumerable().ConfigureAwait(false))
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
                join p in repo.Products on c.Id equals p.ProductCategoryId
                join i in repo.OrderItems on p.Id equals i.ProductId
                group new { c, p, i } by c.Name into g
                select new
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.i.Quantity * x.p.Price),
                };
            await foreach (var item in totalAmountByCategoryQuery.AsAsyncEnumerable().ConfigureAwait(false))
            {
                PrintLine($"  {item}");
            }
        }
    }
}
