// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Linq;
    using Xunit;

    public class When_running_query
    {
        private readonly IQueryable<TestData.Category> _categoryQueriable;
        private readonly IQueryable<TestData.Product> _productQueriable;

        public When_running_query()
        {
            var dataStore = new TestData.Store();
            _categoryQueriable = RemoteQueryable.Create<TestData.Category>(x => x.Execute(queryableProvider: dataStore.Get));
            _productQueriable = RemoteQueryable.Create<TestData.Product>(x => x.Execute(queryableProvider: dataStore.Get));
        }

        [Fact]
        public void Should_return_all_product()
        {
            var result = _productQueriable.ToList();

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
    }
}