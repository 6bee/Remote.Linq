// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Remote.Linq.Tests.RemoteQueryable.QueryTestData;
    using Remote.Linq.Tests.Serialization;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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

        public class Item<T>
        {
            public Item()
            {
            }

            public Item(T value)
            {
                Value = value;
            }

            public T Value { get; set; }
        }

        private readonly IQueryable<Category> _categoryQueryable;
        private readonly IQueryable<Product> _productQueryable;
        private readonly IQueryable<OrderItem> _orderItemQueryable;

        protected When_running_query(Func<Expression, Expression> serialize)
        {
            Store dataStore = new Store();
            Func<Expression, IEnumerable<DynamicObject>> execute =
                (expression) => serialize(expression).Execute(queryableProvider: dataStore.Get);

            _categoryQueryable = RemoteQueryable.Create<Category>(execute);
            _productQueryable = RemoteQueryable.Create<Product>(execute);
            _orderItemQueryable = RemoteQueryable.Create<OrderItem>(execute);
        }

        [Fact]
        public void Should_return_all_product()
        {
            var result = _productQueryable.ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_all_product_using_typeis_filter()
        {
            var result = _productQueryable.Where(p => p is Product).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_all_product_using_typeas_projection()
        {
            var result = _productQueryable.Select(p => p as Product).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_products_filtered_by_category()
        {
            var result = (
                from p in _productQueryable
                join c in _categoryQueryable on p.CategoryId equals c.Id
                where c.Name == "Vehicles"
                select p).ToList();

            result.Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_products_grouped_by_id()
        {
            var result = _productQueryable.GroupBy(p => p.Id).ToList();

            result.Count().ShouldBe(5);
            result.ForEach(g => g.Count().ShouldBe(1));
        }

        [Fact]
        public void Should_return_products_grouped_by_category()
        {
            var result = (
                from p in _productQueryable
                group p by p.CategoryId into g
                select g).ToList();

            result.Count().ShouldBe(2);
            result.ElementAt(0).Count().ShouldBe(3);
            result.ElementAt(1).Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_products_grouped_by_category_with_groups_wrapped()
        {
            var result = (
                from p in _productQueryable
                group p by p.CategoryId into g
                select new
                {
                    Group = g
                }
                ).ToList();

            result.Count().ShouldBe(2);
            result.ElementAt(0).Group.Count().ShouldBe(3);
            result.ElementAt(1).Group.Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_products_using_groupedby_and_slectmany()
        {
            var result = _productQueryable
                .GroupBy(x => x.Id)
                .SelectMany(x => x)
                .ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_return_orders_containing_products_of_more_than_one_categrory()
        {
            var orders = (
                from i in _orderItemQueryable
                join p in _productQueryable on i.ProductId equals p.Id
                group new { i, p } by i.OrderId into g
                where g.Select(_ => _.p.CategoryId).Distinct().Count() > 1
                select g
                ).ToList();

            orders.Count().ShouldBe(1);
            orders.ElementAt(0).Count().ShouldBe(2);
        }

        [Fact]
        public void Should_return_orders_joined_with_chars_array_closure()
        {
            char[] array = { 'h', 'e', 'l', 'l', 'o' };
            var joinLocalVariable = (
                from i in _orderItemQueryable
                from s in array
                select new { i, s }
                ).ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_chars_new_array_inline()
        {
            var joinNewArrayInit = (
                from i in _orderItemQueryable
                from s in new[] { 'h', 'e', 'l', 'l', 'o' }
                select new { i, s }
                ).ToList();

            joinNewArrayInit.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_string_variable_closure()
        {
            var hello = "hello";
            var joinLocalVariable = (
                from i in _orderItemQueryable
                from s in hello
                select new { i, s }
                ).ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string_inline()
        {
            var joinConst = (
                from i in _orderItemQueryable
                from s in "hello"
                select new { i, s }
                ).ToList();

            joinConst.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string_closure()
        {
            const string hello = "hello";
            var joinConst = (
                from i in _orderItemQueryable
                from s in hello
                select new { i, s }
                ).ToList();

            joinConst.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_query_categories_filterd_using_local_array_inline()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => new[] { "Vehicles" }.Contains(c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    mc.Method,
                    System.Linq.Expressions.Expression.Constant(new[] { "Vehicles" }),
                    mc.Arguments[1]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => new[] { "Vehicles" }.Contains(c.Name)).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaNewArrayInit).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaConst).Single().Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_query_categories_filterd_using_empty_local_array_inline()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => new string[0].Contains(c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    mc.Method,
                    System.Linq.Expressions.Expression.Constant(new string[0]),
                    mc.Arguments[1]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => new string[0].Contains(c.Name)).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaNewArrayInit).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaConst).ToList().ShouldBeEmpty();
        }

        [Fact]
        public void Should_query_categories_filterd_using_local_list_inline()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => new List<string> { "Vehicles" }.Contains(c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    System.Linq.Expressions.Expression.Constant(new List<string> { "Vehicles" }),
                    mc.Method,
                    mc.Arguments[0]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => new List<string> { "Vehicles" }.Contains(c.Name)).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaNewArrayInit).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaConst).Single().Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_query_categories_filterd_using_empty_local_list_inline()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => new List<string>().Contains(c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    System.Linq.Expressions.Expression.Constant(new List<string>()),
                    mc.Method,
                    mc.Arguments[0]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => new List<string>().Contains(c.Name)).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaNewArrayInit).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaConst).ToList().ShouldBeEmpty();
        }

        [Fact]
        public void Should_query_categories_filterd_using_local_list_inline_with_enumerable_method()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => Enumerable.Contains(new List<string> { "Vehicles" }, c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    mc.Method,
                    System.Linq.Expressions.Expression.Constant(new List<string> { "Vehicles" }),
                    mc.Arguments[1]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => Enumerable.Contains(new List<string> { "Vehicles" }, c.Name)).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaNewArrayInit).Single().Name.ShouldBe("Vehicles");
            _categoryQueryable.Where(lambdaConst).Single().Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_query_categories_filterd_using_empty_local_list_inline_with_enumerable_method()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> lambdaNewArrayInit =
                c => Enumerable.Contains(new List<string>(), c.Name);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaNewArrayInit.Body;

            var lambdaConst = System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Call(
                    mc.Method,
                    System.Linq.Expressions.Expression.Constant(new List<string>()),
                    mc.Arguments[1]),
                lambdaNewArrayInit.Parameters);

            _categoryQueryable.Where(c => Enumerable.Contains(new List<string>(), c.Name)).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaNewArrayInit).ToList().ShouldBeEmpty();
            _categoryQueryable.Where(lambdaConst).ToList().ShouldBeEmpty();
        }

        [Fact]
        public void Should_query_products_filterd_using_local_variables_closure_inline_mix()
        {
            IEnumerable<int?> listOfIds = new List<int?>() { null, 1, 11, 111 };
            int oneId = 10;
            var query =
                from p in _productQueryable
                where listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(4);
        }

        [Fact]
        public void Should_query_products_filterd_using_const_integer_closure()
        {
            const int oneId = 10;
            var query =
                from p in _productQueryable
                where p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_products_filterd_using_anonymous_argument_closure()
        {
            var arg = new { Id = 10.0 };
            var query =
                from p in _productQueryable
                where p.Id == arg.Id
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_return_true_for_querying_any()
        {
            _productQueryable.Any().ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_for_querying_any_with_filter()
        {
            _productQueryable.Any(x => true).ShouldBeTrue();
        }
        
        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        //[MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_query_primitive_value_injected_as_variable_closure(Type type, object value)
        {
            GetType().GetTypeInfo().GetMethods()
                .Single(m => m.Name == nameof(Should_query_primitive_value_injected_as_variable_closure) && m.IsGenericMethod)
                .MakeGenericMethod(type)
                .Invoke(this, new[] { value });
        }

        public void Should_query_primitive_value_injected_as_variable_closure<T>(T value)
        {
            _productQueryable.Select(x => value).ShouldAllBe(x => Equals(x, value), $"type: {typeof(T).FullName}, value: {value}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        //[MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure(Type type, object value)
        {
            GetType().GetTypeInfo().GetMethods()
                .Single(m => m.Name == nameof(Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure) && m.IsGenericMethod)
                .MakeGenericMethod(type)
                .Invoke(this, new[] { value });
        }

        public void Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure<T>(T value)
        {
            _productQueryable.Select(x => new { Value = value }).ShouldAllBe(x => Equals(x.Value, value), $"type: {typeof(T).FullName}, value: {value}");
        }

        [Fact]
        public void Should_throw_on_query_first_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueryable.Where(x => false).First(); });
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_throw_on_query_first_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueryable.First(x => false); });
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_throw_on_query_last_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueryable.Where(x => false).Last(); });
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_throw_on_query_last_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => { _productQueryable.Last(x => false); });
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_throw_on_query_single_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(delegate { _productQueryable.Single(); });
            ex.Message.ShouldBe("Sequence contains more than one element");
        }

        [Fact]
        public void Should_throw_on_query_single_with_filter_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(delegate { _productQueryable.Single(x => x.Name.Length > 0); });
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }
    }
}