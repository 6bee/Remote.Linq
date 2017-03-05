// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.Tests.RemoteQueryable.TestData;
    using Remote.Linq.Tests.Serialization;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public abstract class When_running_query
    {
        public class With_no_serialization : When_running_query
        {
            public With_no_serialization() : base(x => x) { }
        }

        public class With_data_contract_serializer : When_running_query
        {
            public With_data_contract_serializer() : base(DataContractSerializationHelper.Serialize) { }
        }

        public class With_json_serializer : When_running_query
        {
            public With_json_serializer() : base(x => (Expression)JsonSerializationHelper.Serialize(x, x.GetType())) { }
        }

        public class With_xml_serializer : When_running_query
        {
            public With_xml_serializer() : base(XmlSerializationHelper.Serialize) { }
        }

#if NET
        public class With_net_data_contract_serializer : When_running_query
        {
            public With_net_data_contract_serializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }

        public class With_binary_formatter : When_running_query
        {
            public With_binary_formatter() : base(BinarySerializationHelper.Serialize) { }
        }
#endif

        private readonly IQueryable<Category> _categoryQueriable;
        private readonly IQueryable<Product> _productQueriable;
        private readonly IQueryable<OrderItem> _orderItemQueriable;

        protected When_running_query(Func<Expression, Expression> serialize)
        {
            Store dataStore = new Store();
            Func<Expression, IEnumerable<DynamicObject>> execute =
                (expression) => serialize(expression).Execute(queryableProvider: dataStore.Get);

            _categoryQueriable = RemoteQueryable.Create<Category>(execute);
            _productQueriable = RemoteQueryable.Create<Product>(execute);
            _orderItemQueriable = RemoteQueryable.Create<OrderItem>(execute);
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
            var result = _productQueriable.Where(p => p is Product).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_all_product_using_typeas_projection()
        {
            var result = _productQueriable.Select(p => p as Product).ToList();

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

        [Fact]
        public void Should_return_orders_joined_with_chars_array()
        {
            char[] array = { 'h', 'e', 'l', 'l', 'o' };
            var joinLocalVariable = (
                from i in _orderItemQueriable
                from s in array
                select new { i, s }
                ).ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_chars_new_array_init()
        {
            var joinNewArrayInit = (
                from i in _orderItemQueriable
                from s in new[] { 'h', 'e', 'l', 'l', 'o' }
                select new { i, s }
                ).ToList();

            joinNewArrayInit.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_string()
        {
            var hello = "hello";
            var joinLocalVariable = (
                from i in _orderItemQueriable
                from s in hello
                select new { i, s }
                ).ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string()
        {
            var joinConst = (
                from i in _orderItemQueriable
                from s in "hello"
                select new { i, s }
                ).ToList();

            joinConst.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string2()
        {
            const string hello = "hello";
            var joinConst = (
                from i in _orderItemQueriable
                from s in hello
                select new { i, s }
                ).ToList();

            joinConst.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_query_products_filterd_using_local_variables()
        {
            IEnumerable<int?> listOfIds = new List<int?>() { null, 1, 11, 111 };
            int oneId = 10;
            var query =
                from p in _productQueriable
                where listOfIds.Contains(p.Id) || p.Id % 3 == 0 || p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(3);
        }

        [Fact]
        public void Should_query_products_filterd_using_const_integer()
        {
            const int oneId = 10;
            var query =
                from p in _productQueriable
                where p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_products_filterd_using_anonymous_argument()
        {
            var arg = new { Id = 10.0 };
            var query =
                from p in _productQueriable
                where p.Id == arg.Id
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_return_true_for_querying_any()
        {
            _productQueriable.Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_for_querying_any_with_filter()
        {
            _productQueriable.Any(x => true).ShouldBeTrue();
        }

        [Fact]
        public void Should_throw_on_query_first_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueriable.Where(x => false).First(); });
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_throw_on_query_first_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueriable.First(x => false); });
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_throw_on_query_last_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueriable.Where(x => false).Last(); });
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_throw_on_query_last_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueriable.Last(x => false); });
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_throw_on_query_single_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(delegate { _productQueriable.Single(); });
            ex.Message.ShouldBe("Sequence contains more than one element");
        }

        [Fact]
        public void Should_throw_on_query_single_with_filter_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(delegate { _productQueriable.Single(x => x.Name.Length > 0); });
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }
    }
}