// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Aqua.Dynamic;
    using Aqua.Extensions;
    using Aqua.TypeSystem;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Remote.Linq.Tests.RemoteQueryable.QueryTestData;
    using Remote.Linq.Tests.Serialization;
    using Shouldly;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Xunit;

    public abstract class When_running_query
    {
#pragma warning disable SA1502 // Element should not be on a single line
#pragma warning disable SA1128 // Put constructor initializers on their own line

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

        public class With_binary_formatter : When_running_query
        {
            public With_binary_formatter() : base(BinarySerializationHelper.Serialize) { }
        }

#if NET
        public class With_net_data_contract_serializer : When_running_query
        {
            public With_net_data_contract_serializer() : base(NetDataContractSerializationHelper.Serialize) { }
        }
#endif

#pragma warning restore SA1128 // Put constructor initializers on their own line
#pragma warning restore SA1502 // Element should not be on a single line

        private readonly IQueryable<Category> _categoryQueryable;
        private readonly IQueryable<Product> _productQueryable;
        private readonly IQueryable<Order> _orderQueryable;
        private readonly IQueryable<OrderItem> _orderItemQueryable;
        private int _roundtripCount = 0;

        protected When_running_query(Func<Expression, Expression> serialize)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            Store dataStore = new Store();
            Func<Expression, IEnumerable<DynamicObject>> execute = expression =>
                {
                    Interlocked.Increment(ref _roundtripCount);
                    return serialize(expression).Execute(queryableProvider: dataStore.Get);
                };

            _categoryQueryable = RemoteQueryable.Factory.CreateQueryable<Category>(execute, canBeEvaluatedLocally: CanBeEvaluated);
            _productQueryable = RemoteQueryable.Factory.CreateQueryable<Product>(execute, canBeEvaluatedLocally: CanBeEvaluated);
            _orderQueryable = RemoteQueryable.Factory.CreateQueryable<Order>(execute, canBeEvaluatedLocally: CanBeEvaluated);
            _orderItemQueryable = RemoteQueryable.Factory.CreateQueryable<OrderItem>(execute, canBeEvaluatedLocally: CanBeEvaluated);
        }

        private static bool CanBeEvaluated(System.Linq.Expressions.Expression expression)
        {
            var dbFunctionsProperty = typeof(Db).GetProperty(nameof(Db.Functions), BindingFlags.Static | BindingFlags.Public);

            if (expression.NodeType == System.Linq.Expressions.ExpressionType.MemberAccess)
            {
                var member = ((System.Linq.Expressions.MemberExpression)expression).Member;
                if (member == dbFunctionsProperty)
                {
                    return false;
                }
            }

            return true;
        }

        [Fact]
        public void Should_return_all_product()
        {
            var result = _productQueryable.ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_execute_as_untyped_queryable()
        {
            var count = 0;
            var queryable = (IQueryable)_productQueryable;

            foreach (object item in queryable)
            {
                item.ShouldBeOfType<Product>();
                count++;
            }

            count.ShouldBe(5);
        }

        [Fact]
        public void Should_execute_as_untyped_enumerable()
        {
            var count = 0;
            var enumerable = (IEnumerable)_productQueryable;

            foreach (object item in enumerable)
            {
                item.ShouldBeOfType<Product>();
                count++;
            }

            count.ShouldBe(5);
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
                    Group = g,
                })
                .ToList();

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
                select g)
                .ToList();

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
                select new { i, s })
                .ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_chars_new_array_inline()
        {
            var joinNewArrayInit = (
                from i in _orderItemQueryable
                from s in new[] { 'h', 'e', 'l', 'l', 'o' }
                select new { i, s })
                .ToList();

            joinNewArrayInit.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_string_variable_closure()
        {
            var hello = "hello";
            var joinLocalVariable = (
                from i in _orderItemQueryable
                from s in hello
                select new { i, s })
                .ToList();

            joinLocalVariable.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string_inline()
        {
            var joinConst = (
                from i in _orderItemQueryable
                from s in "hello"
                select new { i, s })
                .ToList();

            joinConst.Count.ShouldBe(15);
        }

        [Fact]
        public void Should_return_orders_joined_with_const_string_closure()
        {
            const string hello = "hello";
            var joinConst = (
                from i in _orderItemQueryable
                from s in hello
                select new { i, s })
                .ToList();

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
        public void Should_query_products_filterd_using_enum_inline()
        {
            // p => p.PruductTags.HasFlag(Convert(BestPrice))
            System.Linq.Expressions.Expression<Func<Product, bool>> lambdaConvertEnum =
                p => p.PruductTags.HasFlag(PruductTags.TopSelling);

            var mc = (System.Linq.Expressions.MethodCallExpression)lambdaConvertEnum.Body;

            // p => p.PruductTags.HasFlag(Const(BestPrice))
            var lambdaConstEnum = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(
                System.Linq.Expressions.Expression.Call(
                    mc.Object,
                    mc.Method,
                    System.Linq.Expressions.Expression.Constant(PruductTags.BestPrice, typeof(Enum))),
                lambdaConvertEnum.Parameters);

            _productQueryable.Where(lambdaConvertEnum).ToList().Count().ShouldBe(1);
            _productQueryable.Where(lambdaConstEnum).ToList().Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_category_filterd_using_local_int_variable()
        {
            int id = 1;
            var query =
                from x in _categoryQueryable
                where x.Id == id
                select x;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_category_filterd_using_local_short_variable()
        {
            short id = 1; // potential issue: the test doesn't seem to cover the circumstance of the unknown id variable/arg type (inline type)
            var query =
                from x in _categoryQueryable
                where x.Id == id
                select x;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
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
        public void Should_query_products_filterd_using_local_variables_closure_inline_mix_with_EnumerableQuery()
        {
            IQueryable<int?> listOfIds = new List<int?>() { null, 1, 11, 111 }.AsQueryable(); // <=== EnumerableQuery
            int oneId = 10;
            var query =
                from p in _productQueryable
                where listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(4);
        }

        private static IQueryable<int?> __listOfIds = new List<int?>() { null, 1, 11, 111 }.AsQueryable(); // <=== EnumerableQuery

        [Fact]
        public void Should_query_products_filterd_using_local_variables_closure_inline_mix_with_EnumerableQuery2()
        {
            int oneId = 10;
            var query =
                from p in _productQueryable
                where __listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(4);
        }

        [Fact]
        public void Should_join_remote_query_with_EnumerableQuery()
        {
            IQueryable<int?> factors = new List<int?>() { null, 1, 2, 3 }.AsQueryable(); // <=== EnumerableQuery
            var query =
                from p in _productQueryable.Select(x => x.Price)
                from f in factors
                select p * (f ?? 0);
            var result = query.ToArray();
            result.Count().ShouldBe(5 * 4);
        }

        private static IQueryable<int?> __factors = new List<int?>() { null, 1, 2, 3 }.AsQueryable(); // <=== EnumerableQuery

        [Fact]
        public void Should_join_remote_query_with_EnumerableQuery2()
        {
            var query =
                from p in _productQueryable.Select(x => x.Price)
                from f in __factors
                select p * (f ?? 0);
            var result = query.ToArray();
            result.Count().ShouldBe(5 * 4);
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
        public void Should_query_product_categories_filterd_using_inline_enum()
        {
            var query =
                from c in _categoryQueryable
                where c.CategoryType == CategoryType.NonFood
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_enum_equals()
        {
            var query =
                from c in _categoryQueryable
                where c.CategoryType.Equals(CategoryType.NonFood)
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_enum_closure()
        {
            var categoryType = CategoryType.NonFood;
            var query =
                from c in _categoryQueryable
                where c.CategoryType == categoryType
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum()
        {
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType == CategorySourceType.Internal
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals()
        {
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType.Equals(CategorySourceType.Internal)
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_nullable_enum_closure()
        {
            var categoryType = CategorySourceType.Internal;
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType == categoryType
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum_is_null()
        {
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType == null
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals_is_null()
        {
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType.Equals(null)
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_nullable_enum_is_null_closure()
        {
            CategorySourceType? categoryType = null;
            var query =
                from c in _categoryQueryable
                where c.CategorySourceType == categoryType
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum_reference_is_null()
        {
            var query =
                from c in _categoryQueryable
                where ReferenceEquals(null, c.CategorySourceType)
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals_reference_is_null()
        {
            var query =
                from c in _categoryQueryable
                where ReferenceEquals(null, c.CategorySourceType)
                select c;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_filterd_using_inline_enum_flags()
        {
            var query =
                from p in _productQueryable
                where p.PruductTags.HasFlag(PruductTags.BestPrice)
                select p;
            var result = query.ToArray();
            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_query_product_filterd_using_inline_enum_flags_closure()
        {
            var flag = PruductTags.BestPrice;
            var query =
                from p in _productQueryable
                where p.PruductTags.HasFlag(flag)
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

        [Fact]
        public void Nested_query_should_not_cause_multiple_roundtrips()
        {
            var result = _productQueryable.Where(p => _categoryQueryable.Any() && p.Id >= 1).ToList();
            result.Count.ShouldBe(5);
            _roundtripCount.ShouldBe(1);
        }

        [Fact]
        public void Nested_query_using_local_remote_queryables_should_not_cause_multiple_roundtrips()
        {
            var count = 0;
            Func<Expression, IEnumerable<DynamicObject>> execute = exp =>
            {
                Interlocked.Increment(ref count);
                var dataprovider = new Dictionary<Type, System.Collections.IEnumerable>()
                {
                    { typeof(Category), new[] { new Category() } },
                    { typeof(Product), new[] { new Product { Id = 0 }, new Product { Id = 1 }, new Product { Id = 2 } } },
                };
                return exp.Execute(t => dataprovider[t].AsQueryable());
            };
            var categoryQueryable = RemoteQueryable.Factory.CreateQueryable<Category>(execute);
            var productQueryable = RemoteQueryable.Factory.CreateQueryable<Product>(execute);

            var result = productQueryable.Where(p => categoryQueryable.Any() && p.Id >= 1).ToList();
            result.Count.ShouldBe(2);
            count.ShouldBe(1);
        }

        [Fact]
        public void Nested_query_with_predicate_should_not_cause_multiple_roundtrips()
        {
            _productQueryable
                .Where(p => _categoryQueryable
                    .Where(c => c.Name == "always false")
                    .Any(c => c.Id == p.CategoryId))
                .ToList()
                .ShouldBeEmpty();

            _roundtripCount.ShouldBe(1);
        }

        [Fact]
        public void Nested_query_using_local_predicate_should_not_cause_multiple_roundtrips()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> predicate =
                c => c.Name == "always false";

            _productQueryable
                .Where(p => _categoryQueryable.Any(predicate))
                .ToList()
                .ShouldBeEmpty();

            _roundtripCount.ShouldBe(1);
        }

        [Fact]
        public void Should_support_combination_of_take_and_select_many()
        {
            var result = (
               from p in _productQueryable.Take(1)
               from c in _categoryQueryable.Take(1)
               select new { p, c }).ToList();

            result.Count().ShouldBe(1);
        }

        [Fact]
        public void Should_not_try_evaluate_a_subexpression_to_find_queryables_if_it_accesses_any_parameter()
        {
            _productQueryable
                .Select(p => new { ids = _productQueryable.Select(pp => pp.Id) })
                .Select(a => a.ids.Count())
                .ToList();
        }

        [Fact]
        public void Should_allow_filtering_on_null_property_within_nested_query()
        {
            var result = _productQueryable
                .Where(p =>
                    _orderQueryable.Any(o =>
                        _orderItemQueryable.Any(i =>
                            o.Items.Contains(i) && o.ShippingAddress != null && i.ProductId == p.Id)))
                .Select(x => x.Id)
                .ToList();

            result.Count.ShouldBe(2);
            result.ShouldContain(11);
            result.ShouldContain(14);

            _roundtripCount.ShouldBe(1);
        }

        [Fact]
        public void Should_allow_filtering_using_invokeexpression()
        {
            System.Linq.Expressions.Expression<Func<Category, bool>> expression = c => c.Name == "hello";
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Category), "c");

            _categoryQueryable.Where(
                System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                    System.Linq.Expressions.Expression.Invoke(expression, parameter), parameter)).ToList();
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        public void Should_query_primitive_value_injected_as_variable_closure(Type type, object value)
        {
            RunTestMethod(
                nameof(TestMethodFor_Should_query_primitive_value_injected_as_variable_closure),
                type,
                value);
        }

        protected void TestMethodFor_Should_query_primitive_value_injected_as_variable_closure<T>(T value)
        {
            _productQueryable.Select(x => value).ShouldAllBe(x => Equals(x, value), $"type: {typeof(T).FullName}, value: {value}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_query_primitive_value_array_injected_as_variable_closure(Type type, object value)
        {
            RunTestMethod(
                nameof(TestMethodFor_Should_query_primitive_value_array_injected_as_variable_closure),
                TypeHelper.GetElementType(type),
                value);
        }

        protected void TestMethodFor_Should_query_primitive_value_array_injected_as_variable_closure<T>(T[] array)
        {
            _productQueryable.Select(x => array).ShouldAllBe(x => x.CollectionEquals(array), $"element type: {typeof(T).FullName}, array: {array}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValues), MemberType = typeof(TestData))]
        public void Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure(Type type, object value)
        {
            RunTestMethod(
                nameof(TestMethodFor_Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure),
                type,
                value);
        }

        protected void TestMethodFor_Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure<T>(T value)
        {
            _productQueryable.Select(x => new { Value = value }).ShouldAllBe(x => Equals(x.Value, value), $"type: {typeof(T).FullName}, value: {value}");
        }

        [Theory]
        [MemberData(nameof(TestData.PrimitiveValueArrays), MemberType = typeof(TestData))]
        public void Should_query_anonymous_type_with_primitive_value_array_injected_as_variable_closure(Type type, object value)
        {
            RunTestMethod(
                nameof(TestMethodFor_Should_query_anonymous_type_with_primitive_value_array_injected_as_variable_closure),
                TypeHelper.GetElementType(type),
                value);
        }

        protected void TestMethodFor_Should_query_anonymous_type_with_primitive_value_array_injected_as_variable_closure<T>(T[] array)
        {
            _productQueryable.Select(x => new { Array = array }).ShouldAllBe(x => x.Array.CollectionEquals(array), $"element type: {typeof(T).FullName}, array: {array}");
        }

        private void RunTestMethod(string methodName, Type genericType, object argument)
        {
            GetType()
                .GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(genericType)
                .Invoke(this, new[] { argument });
        }

        [SkippableFact]
        public void Should_query_value_created_using_default_constructor()
        {
            Skip.If(this.TestIs<With_xml_serializer>(), "Not supported by XmlSerializer (circuar reference)");

            _productQueryable
                .Select(x => default(DateTime))
                .ShouldAllBe(x => Equals(x, default(DateTime)));
        }

        [Fact]
        public void Should_not_evaluate_DB_functions_prematurely()
        {
            var result = _productQueryable.Where(p => Db.Functions.Like(p.Name, "%")).ToList();

            result.Count().ShouldBe(5);
        }

        [Fact]
        public void Should_throw_on_query_first_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.Where(x => false).First());
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_return_on_query_first_or_default_on_empty_sequence()
        {
            _productQueryable.Where(x => false).FirstOrDefault().ShouldBeNull();
        }

        [Fact]
        public void Should_throw_on_query_first_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.First(x => false));
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_return_null_on_query_first_or_default_with_filter_on_empty_sequence()
        {
            _productQueryable.FirstOrDefault(x => false).ShouldBeNull();
        }

        [Fact]
        public void Should_throw_on_query_last_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.Where(x => false).Last());
            ex.Message.ShouldBe("Sequence contains no elements");
        }

        [Fact]
        public void Should_return_null_on_query_last_or_default_on_empty_sequence()
        {
            _productQueryable.Where(x => false).LastOrDefault().ShouldBeNull();
        }

        [Fact]
        public void Should_throw_on_query_last_with_filter_on_empty_sequence()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.Last(x => false));
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void Should_return_null_on_query_last_or_default_with_filter_on_empty_sequence()
        {
            _productQueryable.LastOrDefault(x => false).ShouldBeNull();
        }

        [Fact]
        public void Should_throw_on_query_single_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.Single());
            ex.Message.ShouldBe("Sequence contains more than one element");
        }

        [Fact]
        public void Should_throw_on_query_single_with_filter_if_more_than_one_element()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.Single(x => x.Name.Length > 0));
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }

        [Fact]
        public void Should_throw_on_query_elementat_with_index_out_of_bounds()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => _productQueryable.ElementAt(10));
            ex.Message.ShouldStartWith("Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        [Fact]
        public void Should_return_null_on_query_elementat_or_default_with_index_out_of_bounds()
        {
            _productQueryable.ElementAtOrDefault(10).ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_on_query_elementat_or_default_with_negative_index()
        {
            _productQueryable.ElementAtOrDefault(-1).ShouldBeNull();
        }

        [Fact]
        public void Should_support_non_materialized_ienumerable_query_argument()
        {
            var data = new List<(int key, string value)>
            {
                (1, "Fruit"),
                (2, "Vehicle"),
            };
            var filteredItems = data
                .Where(x => x.value.StartsWith("V"))
                .Select(x => x.key);
            var result = _categoryQueryable.FirstOrDefault(x => filteredItems.Contains(x.Id));
            result.Id.ShouldBe(2);
            result.Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_support_non_materialized_enumerablequery_argument()
        {
            var data = new List<(int key, string value)>
            {
                (1, "Fruit"),
                (2, "Vehicle"),
            };
            var filteredItems = data
                .AsQueryable()
                .Where(x => x.value.StartsWith("V"))
                .Select(x => x.key);
            var result = _categoryQueryable.FirstOrDefault(x => filteredItems.Contains(x.Id));
            result.Id.ShouldBe(2);
            result.Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_support_tuple_query_argument()
        {
            var data = (2, "Vehicle");
            var result = _categoryQueryable.FirstOrDefault(x => x.Id == data.Item1);
            result.Id.ShouldBe(2);
            result.Name.ShouldBe("Vehicles");
        }

        [Fact]
        public void Should_support_string_query_argument()
        {
            var data = "Vehicle";
            var result = _categoryQueryable.FirstOrDefault(x => x.Name.StartsWith(data));
            result.Id.ShouldBe(2);
            result.Name.ShouldBe("Vehicles");
        }
    }
}