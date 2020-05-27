// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Common.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static CommonHelper;

    public class Demo
    {
        private readonly Func<IRemoteRepository> _repoProvider;

        public Demo(Func<IRemoteRepository> repoProvider)
        {
            _repoProvider = repoProvider;
        }

        public void Run()
        {
            using IRemoteRepository repo = _repoProvider();

            PrintHeader("GET ALL PRODUCTS:");
            foreach (var item in repo.Products)
            {
                PrintLine($"  {item.Id} | {item.Name} | {item.Price:C}");
            }

            PrintHeader("GET PRODUCTS FILTERED BY ID:");
            List<int> idSelection = new List<int> { 1, 11, 111 };
            foreach (var item in repo.Products.Where(p => idSelection.Contains(p.Id) || p.Id % 3 == 0))
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
#if KNOW_TYPES_ONLY
                            .Select(x => new CrossJoinResultDto { Category = x.Category, Name = x.Name })
#endif
                            .ToList();

            foreach (var i in crossJoinResult)
            {
                PrintLine($"  {i}");
            }

            PrintHeader("INNER JOIN:");
            var innerJoinQuery =
                from c in repo.ProductCategories
                join p in repo.Products on c.Id equals p.ProductCategoryId
                select new { c.Name, P = new { p.Price }, X = new { Y = string.Concat(c.Name, "-", p.Name) } };
            var innerJoinResult = innerJoinQuery
#if KNOW_TYPES_ONLY
                .Select(x => new InnerJoinResultDto { Name = x.Name, Price = x.P.Price, XY = x.X.Y })
#endif
                .ToList();

            foreach (var item in innerJoinResult)
            {
                PrintLine($"  {item}");
            }

            PrintHeader("SELECT IDs:");
            IQueryable<int> productIdsQuery =
                from p in repo.Products
                orderby p.Price descending
                select p.Id;
            List<int> productIds = productIdsQuery.ToList();
            foreach (int id in productIdsQuery)
            {
                PrintLine($"  {id}");
            }

            PrintHeader("COUNT:");
            IQueryable<Product> productsQuery =
                from p in repo.Products
                select p;
            PrintLine($"  Count = {productsQuery.Count()}");

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
                };

            var totalAmountByCategroyResult = totalAmountByCategoryQuery
#if KNOW_TYPES_ONLY
                .Select(x => new TotalAmountByCategoryDto { Category = x.Category, Amount = x.Amount })
#endif
                .ToDictionary(x => x.Category, x => x.Amount);

            foreach (var i in totalAmountByCategroyResult)
            {
                PrintLine($"  {i.Key}: {i.Value:C}");
            }

            PrintHeader("GET PRODUCT GROUPS:");
            foreach (ProductGroup group in repo.ProductGroups)
            {
                PrintLine($"  {group.Id} | {group.GroupName}");

                foreach (Product p in group.Products)
                {
                    PrintLine($"    | * {p.Name}");
                }
            }

            PrintHeader("EXPECTED INVALID OPERATION:");
            try
            {
                _ = totalAmountByCategoryQuery
                    .Select(x => x.Category)
                    .First(x => false);
            }
            catch (InvalidOperationException ex)
            {
                PrintLine($"  {ex.Message}");
            }
        }
    }
}