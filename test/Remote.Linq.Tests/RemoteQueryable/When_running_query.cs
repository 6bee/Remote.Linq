// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System.Linq;
    using Xunit;

    public class When_running_query
    {
        private readonly IQueryable<TestData.Category> _categoryQueriable;
        private readonly IQueryable<TestData.Product> _productQueriable;
        private readonly IQueryable<TestData.OrderItem> _orderItemQueriable;

        public When_running_query()
        {
            var dataStore = new TestData.Store();
            _categoryQueriable = RemoteQueryable.Create<TestData.Category>(x => x.Execute(queryableProvider: dataStore.Get));
            _productQueriable = RemoteQueryable.Create<TestData.Product>(x => x.Execute(queryableProvider: dataStore.Get));
            _orderItemQueriable = RemoteQueryable.Create<TestData.OrderItem>(x => x.Execute(queryableProvider: dataStore.Get));
        }

        [Fact]
        public void Should_return_all_product()
        {
            var result = _productQueriable.ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_all_product_using_typeis_filter()
        {
            var result = _productQueriable.Where(p => p is TestData.Product).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_all_product_using_typeas_projection()
        {
            var result = _productQueriable.Select(p => p as TestData.Product).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_products_filtered_by_category()
        {
            var result = (
                from p in _productQueriable
                join c in _categoryQueriable on p.CategoryId equals c.Id
                where c.Name == "Vehicles"
                select p).ToList();

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_products_grouped_by_id()
        {
            var result = _productQueriable.GroupBy(p => p.Id).ToList();

            result.Count().ShouldBe(5);
            result.ForEach(g => g.Count().ShouldBe(1));
        }

        [Fact]
        public void Should_return_products_grouped_by_category()
        {
            var result = (
                from p in _productQueriable
                group p by p.CategoryId into g
                select g).ToList();

            result.Count().ShouldBe(2);
            result.ElementAt(0).Count().ShouldBe(3);
            result.ElementAt(1).Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_products_using_groupedby_and_slectmany()
        {
            var result = _productQueriable
                .GroupBy(x => x.Id)
                .SelectMany(x => x)
                .ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_orders_containing_products_of_more_than_one_categrory()
        {
            var orders = (
                from i in _orderItemQueriable
                join p in _productQueriable on i.ProductId equals p.Id
                group new { i, p } by i.OrderId into g
                where g.Select(_ => _.p.CategoryId).Distinct().Count() > 1
                select g
                ).ToList();

            orders.Count().ShouldBe(1);
            orders.ElementAt(0).Count().ShouldBe(2);
        }
    }
}