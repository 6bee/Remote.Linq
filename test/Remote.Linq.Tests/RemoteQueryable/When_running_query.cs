﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable;

using Aqua.Dynamic;
using Aqua.EnumerableExtensions;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Remote.Linq.Tests.RemoteQueryable.QueryTestData;
using Remote.Linq.Tests.Serialization;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xunit;

public abstract class When_running_query
{
    public class With_no_serialization : When_running_query
    {
        public With_no_serialization()
            : base(x => x)
        {
        }
    }

    public class With_data_contract_serializer : When_running_query
    {
        public With_data_contract_serializer()
            : base(DataContractSerializationHelper.Clone)
        {
        }
    }

    public class With_newtonsoft_json_serializer : When_running_query
    {
        public With_newtonsoft_json_serializer()
            : base(x => (Expression)NewtonsoftJsonSerializationHelper.Clone(x, x.GetType()))
        {
        }
    }

    public class With_system_text_json_serializer : When_running_query
    {
        public With_system_text_json_serializer()
            : base(x => (Expression)SystemTextJsonSerializationHelper.Clone(x, x.GetType()))
        {
        }
    }

    public class With_xml_serializer : When_running_query
    {
        public With_xml_serializer()
            : base(XmlSerializationHelper.Clone)
        {
        }
    }

#if !NET8_0_OR_GREATER
    public class With_binary_formatter : When_running_query
    {
        public With_binary_formatter()
            : base(BinarySerializationHelper.Clone)
        {
        }
    }
#endif // NET8_0_OR_GREATER

    public class With_protobuf_net_serializer : When_running_query
    {
        public With_protobuf_net_serializer()
            : base(ProtobufNetSerializationHelper.Clone)
        {
        }
    }

#if NETFRAMEWORK
    public class With_net_data_contract_serializer : When_running_query
    {
        public With_net_data_contract_serializer()
            : base(NetDataContractSerializationHelper.Clone)
        {
        }
    }
#endif // NETFRAMEWORK

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
        Func<Expression, DynamicObject> execute = expression =>
            {
                Interlocked.Increment(ref _roundtripCount);
                var dynamicObject = serialize(expression).Execute(queryableProvider: dataStore.Get);

                // set anonymous root type to unknown type to simulate dynamically emitted type
                if (dynamicObject?.Type?.IsAnonymousType is true)
                {
                    dynamicObject.Type.Name = $"DYNAMICALLY_EMITTED_{dynamicObject.Type.Name}";
                }

                return dynamicObject;
            };

        _categoryQueryable = RemoteQueryable.Factory.CreateQueryable<Category>(execute, null, canBeEvaluatedLocally: CanBeEvaluated);
        _productQueryable = RemoteQueryable.Factory.CreateQueryable<Product>(execute, null, canBeEvaluatedLocally: CanBeEvaluated);
        _orderQueryable = RemoteQueryable.Factory.CreateQueryable<Order>(execute, null, canBeEvaluatedLocally: CanBeEvaluated);
        _orderItemQueryable = RemoteQueryable.Factory.CreateQueryable<OrderItem>(execute, null, canBeEvaluatedLocally: CanBeEvaluated);
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

        result.Count.ShouldBe(5);
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
    [SuppressMessage("Minor Code Smell", "S2219:Runtime type checking should be simplified", Justification = "Intentional test setup")]
    public void Should_return_all_product_using_typeis_filter()
    {
        var result = _productQueryable.Where(p => p is Product).ToList();

        result.Count.ShouldBe(5);
    }

    [Fact]
    [SuppressMessage("Minor Code Smell", "S1905:Redundant casts should not be used", Justification = "Intentional test setup")]
    public void Should_return_all_product_using_typeas_projection()
    {
        var result = _productQueryable.Select(p => p as Product).ToList();

        result.Count.ShouldBe(5);
    }

    [Fact]
    public void Should_return_products_filtered_by_category()
    {
        var result = (
            from p in _productQueryable
            join c in _categoryQueryable on p.CategoryId equals c.Id
            where c.Name == "Vehicles"
            select p).ToList();

        result.Count.ShouldBe(2);
    }

    [Fact]
    public void Should_return_products_grouped_by_id()
    {
        var result = _productQueryable.GroupBy(p => p.Id).ToList();

        result.Count.ShouldBe(5);
        result.ForEach(g => g.Count().ShouldBe(1));
    }

    [Fact]
    public void Should_return_products_grouped_by_category()
    {
        var result = (
            from p in _productQueryable
            group p by p.CategoryId into g
            select g).ToList();

        result.Count.ShouldBe(2);
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

        result.Count.ShouldBe(2);
        result.ElementAt(0).Group.Count().ShouldBe(3);
        result.ElementAt(1).Group.Count().ShouldBe(2);
    }

    [Fact]
    public void Should_return_products_using_groupby_and_selectmany()
    {
        var result = _productQueryable
            .GroupBy(x => x.Id)
            .SelectMany(x => x)
            .ToList();

        result.Count.ShouldBe(5);
    }

    [Fact]
    public void Should_project_into_array_using_array_init()
    {
        var result = _productQueryable
            .Select(p => new[] { p })
            .ToList();

        result.Count.ShouldBe(5);
        result.ForEach(x => x.ShouldHaveSingleItem().ShouldNotBeNull());
    }

    [Fact]
    public void Should_project_into_list_using_list_init()
    {
        var result = _productQueryable
            .Select(p => new List<Product> { p })
            .ToList();

        result.Count.ShouldBe(5);
        result.ForEach(x => x.ShouldHaveSingleItem().ShouldNotBeNull());
    }

    public class MemberBindingTestType
    {
        public Product Product { get; set; } = new Product();

        public List<Product> ProductList { get; set; } = new List<Product>() { new Product() };
    }

    [Fact]
    public void Should_project_using_member_binding()
    {
        var result = _productQueryable
            .Select(p => new MemberBindingTestType { Product = p })
            .ToList();

        result.Count.ShouldBe(5);
        result.ForEach(x => x.Product.Name.ShouldNotBeNullOrWhiteSpace());
    }

    [Fact]
    public void Should_project_using_member_member_binding()
    {
        var result = _productQueryable
            .Select(p => new MemberBindingTestType { Product = { Name = p.Name } })
            .ToList();

        result.Count.ShouldBe(5);
        result.ForEach(x => x.Product.Name.ShouldNotBeNullOrWhiteSpace());
    }

    [Fact]
    public void Should_project_using_member_list_binding()
    {
        var result = _productQueryable
            .Select(p => new MemberBindingTestType { ProductList = { p, p } })
            .ToList();

        result.Count.ShouldBe(5);
        result.ForEach(x => x.ProductList.Count.ShouldBe(3));
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

        orders.Count.ShouldBe(1);
        orders.ElementAt(0).Count().ShouldBe(2);
    }

    [Fact]
    public void Should_return_orders_joined_with_chars_array_closure()
    {
        char[] array = ['h', 'e', 'l', 'l', 'o'];
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

        _productQueryable.Where(lambdaConvertEnum).ToList().Count.ShouldBe(1);
        _productQueryable.Where(lambdaConstEnum).ToList().Count.ShouldBe(1);
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
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_products_filterd_using_local_variables_closure_inline_mix()
    {
        IEnumerable<int?> listOfIds = new List<int?> { null, 1, 11, 111 };
        int oneId = 10;
        var query =
            from p in _productQueryable
            where listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
            select p;
        var result = query.ToArray();
        result.Length.ShouldBe(4);
    }

    [Fact]
    public void Should_query_products_filterd_using_local_variables_closure_inline_mix_with_EnumerableQuery()
    {
        IQueryable<int?> listOfIds = new List<int?> { null, 1, 11, 111 }.AsQueryable(); // <=== EnumerableQuery
        int oneId = 10;
        var query =
            from p in _productQueryable
            where listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
            select p;
        var result = query.ToArray();
        result.Length.ShouldBe(4);
    }

    private static readonly IQueryable<int?> __listOfIds = new List<int?> { null, 1, 11, 111 }.AsQueryable(); // <=== EnumerableQuery

    [Fact]
    public void Should_query_products_filterd_using_local_variables_closure_inline_mix_with_EnumerableQuery2()
    {
        int oneId = 10;
        var query =
            from p in _productQueryable
            where __listOfIds.Contains(p.Id) || new List<string> { "Truck", "Bicycle" }.Contains(p.Name) || p.Id % 3 == 0 || p.Id == oneId
            select p;
        var result = query.ToArray();
        result.Length.ShouldBe(4);
    }

    [Fact]
    public void Should_join_remote_query_with_EnumerableQuery()
    {
        IQueryable<int?> factors = new List<int?> { null, 1, 2, 3 }.AsQueryable(); // <=== EnumerableQuery
        var query =
            from p in _productQueryable.Select(x => x.Price)
            from f in factors
            select p * (f ?? 0);
        var result = query.ToArray();
        result.Length.ShouldBe(5 * 4);
    }

    private static readonly IQueryable<int?> __factors = new List<int?> { null, 1, 2, 3 }.AsQueryable(); // <=== EnumerableQuery

    [Fact]
    public void Should_join_remote_query_with_EnumerableQuery2()
    {
        var query =
            from p in _productQueryable.Select(x => x.Price)
            from f in __factors
            select p * (f ?? 0);
        var result = query.ToArray();
        result.Length.ShouldBe(5 * 4);
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
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_enum()
    {
        var query =
            from c in _categoryQueryable
            where c.CategoryType == CategoryType.NonFood
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_enum_equals()
    {
        var query =
            from c in _categoryQueryable
            where c.CategoryType.Equals(CategoryType.NonFood)
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum()
    {
        var query =
            from c in _categoryQueryable
            where c.CategorySourceType == CategorySourceType.Internal
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals()
    {
        var query =
            from c in _categoryQueryable
            where c.CategorySourceType.Equals(CategorySourceType.Internal)
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum_is_null()
    {
        var query =
            from c in _categoryQueryable
            where c.CategorySourceType == null
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals_is_null()
    {
        var query =
            from c in _categoryQueryable
            where c.CategorySourceType.Equals(null)
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
    }

    [Fact]
    [SuppressMessage("Major Bug", "S2995:\"Object.ReferenceEquals\" should not be used for value types", Justification = "Intentional test setup")]
    [SuppressMessage("Reliability", "CA2013:Do not use ReferenceEquals with value types", Justification = "Intentional test setup")]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum_reference_equals_null()
    {
        var query =
            from c in _categoryQueryable
            where ReferenceEquals(null, c.CategorySourceType)
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_categories_filterd_using_inline_nullable_enum_equals_null()
    {
        var query =
            from c in _categoryQueryable
            where Equals(null, c.CategorySourceType)
            select c;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
    }

    [Fact]
    public void Should_query_product_filterd_using_inline_enum_flags()
    {
        var query =
            from p in _productQueryable
            where p.PruductTags.HasFlag(PruductTags.BestPrice)
            select p;
        var result = query.ToArray();
        result.Length.ShouldBe(1);
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
        result.Length.ShouldBe(1);
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
        Func<Expression, DynamicObject> execute = exp =>
        {
            Interlocked.Increment(ref count);
            var dataprovider = new Dictionary<Type, IEnumerable>
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

        result.Count.ShouldBe(1);
    }

    [Fact]
    public void Should_not_try_evaluate_a_subexpression_to_find_queryables_if_it_accesses_any_parameter()
    {
        var counters = _productQueryable
            .Select(p => new { ids = _productQueryable.Select(pp => pp.Id) })
            .Select(a => a.ids.Count())
            .ToList();

        counters.Count.ShouldBe(5);
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
        System.Linq.Expressions.Expression<Func<Category, bool>> expression = c => c.Name == "Vehicles";
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(Category), "c");

        var result = _categoryQueryable.Where(
            System.Linq.Expressions.Expression.Lambda<Func<Category, bool>>(
                System.Linq.Expressions.Expression.Invoke(expression, parameter), parameter)).ToList();

        result.Count.ShouldBe(1);
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
    public void Should_query_primitive_value_injected_as_variable_closure(Type type, object value)
    {
        if (this.TestIs<With_data_contract_serializer>())
        {
            DataContractSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_protobuf_net_serializer>())
        {
            ProtobufNetSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_system_text_json_serializer>())
        {
            SystemTextJsonSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_xml_serializer>())
        {
            XmlSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        RunTestMethod(
            nameof(TestMethodFor_Should_query_primitive_value_injected_as_variable_closure),
            type,
            value);
    }

    protected void TestMethodFor_Should_query_primitive_value_injected_as_variable_closure<T>(T value)
    {
        _productQueryable.Select(_ => value).ShouldAllBe(x => Equals(x, value), $"type: {typeof(T).FullName}, value: {value}");
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValueArrays), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueLists), MemberType = typeof(TestData))]
    public void Should_query_primitive_value_collection_injected_as_variable_closure(Type type, object value)
    {
        if (this.TestIs<With_data_contract_serializer>())
        {
            DataContractSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_protobuf_net_serializer>())
        {
            ProtobufNetSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_system_text_json_serializer>())
        {
            SystemTextJsonSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_xml_serializer>())
        {
            XmlSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        RunTestMethod(
            nameof(TestMethodFor_Should_query_primitive_value_collection_injected_as_variable_closure),
            TypeHelper.GetElementType(type),
            value);
    }

    protected void TestMethodFor_Should_query_primitive_value_collection_injected_as_variable_closure<T>(IEnumerable<T> collection)
    {
        _productQueryable.Select(_ => collection).ShouldAllBe(x => x.CollectionEquals(collection), $"element type: {typeof(T).FullName}, array: {collection}");
    }

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValues), MemberType = typeof(TestData))]
    public void Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure(Type type, object value)
    {
        if (this.TestIs<With_data_contract_serializer>())
        {
            DataContractSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_protobuf_net_serializer>())
        {
            ProtobufNetSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_system_text_json_serializer>())
        {
            SystemTextJsonSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_xml_serializer>())
        {
            XmlSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        RunTestMethod(
            nameof(TestMethodFor_Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure),
            type,
            value);
    }

    protected void TestMethodFor_Should_query_anonymous_type_with_primitive_value_injected_as_variable_closure<T>(T value)
        => _productQueryable
        .Select(_ => new { Value = value })
        .ShouldAllBe(x => Equals(x.Value, value), $"type: {typeof(T).FullName}, value: {value}");

    [SkippableTheory]
    [MemberData(nameof(TestData.TestValueArrays), MemberType = typeof(TestData))]
    [MemberData(nameof(TestData.TestValueLists), MemberType = typeof(TestData))]
    public void Should_query_anonymous_type_with_primitive_value_collection_injected_as_variable_closure(Type type, object value)
    {
        if (this.TestIs<With_data_contract_serializer>())
        {
            DataContractSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_protobuf_net_serializer>())
        {
            ProtobufNetSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_system_text_json_serializer>())
        {
            SystemTextJsonSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        if (this.TestIs<With_xml_serializer>())
        {
            XmlSerializationHelper.SkipUnsupportedDataType(type, value);
        }

        RunTestMethod(
            nameof(TestMethodFor_Should_query_anonymous_type_with_primitive_value_collection_injected_as_variable_closure),
            TypeHelper.GetElementType(type),
            value);
    }

    protected void TestMethodFor_Should_query_anonymous_type_with_primitive_value_collection_injected_as_variable_closure<T>(IEnumerable<T> collection)
        => _productQueryable
        .Select(_ => new { Collection = collection })
        .ShouldAllBe(x => x.Collection.CollectionEquals(collection), $"element type: {typeof(T).FullName}, array: {collection}");

    private void RunTestMethod(string methodName, Type genericType, object argument)
        => GetType()
        .GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        .MakeGenericMethod(genericType)
        .Invoke(this, [argument]);

    [Fact]
    public void Should_query_value_created_using_default_operator()
    {
        _productQueryable
            .Select(_ => default(DateTime))
            .ShouldAllBe(x => Equals(x, default(DateTime)));
    }

    [Fact]
    public void Should_not_evaluate_DB_functions_prematurely()
    {
        var result = _productQueryable.Where(p => Db.Functions.Like(p.Name, "%")).ToList();

        result.Count.ShouldBe(5);
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
        _productQueryable.Where(_ => false).FirstOrDefault().ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(TestData.TestCultureNames), MemberType = typeof(TestData))]
    public void Should_throw_on_query_first_with_filter_on_empty_sequence(string culture)
    {
        using var cultureContext = TestHelper.CreateCultureContext(culture);

        var ex = Assert.Throws<InvalidOperationException>(() => _productQueryable.First(x => false));
        ex.Message.ShouldBe("Sequence contains no matching element");
    }

    [Fact]
    public void Should_return_null_on_query_first_or_default_with_filter_on_empty_sequence()
    {
        _productQueryable.FirstOrDefault(_ => false).ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(TestData.TestCultureNames), MemberType = typeof(TestData))]
    public void Should_throw_on_query_last_on_empty_sequence(string culture)
    {
        using var cultureContext = TestHelper.CreateCultureContext(culture);

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
        var data = new List<(int Key, string Value)>
        {
            (1, "Fruit"),
            (2, "Vehicle"),
        };
        var filteredItems = data
            .Where(x => x.Value.StartsWith("V"))
            .Select(x => x.Key);
        var result = _categoryQueryable.FirstOrDefault(x => filteredItems.Contains(x.Id));
        result.Id.ShouldBe(2);
        result.Name.ShouldBe("Vehicles");
    }

    [Fact]
    public void Should_support_non_materialized_enumerablequery_argument()
    {
        var data = new List<(int Key, string Value)>
        {
            (1, "Fruit"),
            (2, "Vehicle"),
        };
        var filteredItems = data
            .AsQueryable()
            .Where(x => x.Value.StartsWith("V"))
            .Select(x => x.Key);
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