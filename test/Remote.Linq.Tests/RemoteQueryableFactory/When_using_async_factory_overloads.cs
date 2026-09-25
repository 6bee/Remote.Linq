// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

// Overload-call table for the 31 public CreateAsyncQueryable overloads
// in src/Remote.Linq/RemoteQueryableFactoryAsyncExtensions.cs:
//
//  #  | Overload (provider result / token shape / context shape)                       | Invoked by
//  --- | ----------------------------------------------------------------------------- | ------------------------------------
//   1 | CreateAsyncQueryable(Type, Func<Expr,ValueTask<DO>>, ctx = null, mapper = null) | Should_execute_untyped_dynamic_object_overloads
//   2 | CreateAsyncQueryable(Type, Func<Expr,ValueTask<DO>>, typeInfo, pred = null, …)  | Should_execute_untyped_dynamic_object_overloads
//   3 | CreateAsyncQueryable(Type, Func<Expr,CT,ValueTask<DO>>, ctx = null, mapper = …) | Should_execute_untyped_dynamic_object_overloads
//   4 | CreateAsyncQueryable(Type, Func<Expr,CT,ValueTask<DO>>, typeInfo, pred, mapper) | Should_execute_untyped_dynamic_object_overloads
//   5 | CreateAsyncQueryable<T>(Func<Expr,ValueTask<DO>>, ctx = null, mapper = null)    | Should_execute_typed_dynamic_object_overloads
//   6 | CreateAsyncQueryable<T>(Func<Expr,ValueTask<DO>>, typeInfo, pred = null, …)     | Should_execute_typed_dynamic_object_overloads
//   7 | CreateAsyncQueryable<T>(Func<Expr,CT,ValueTask<DO>>, ctx = null, mapper = null) | Should_execute_typed_dynamic_object_overloads
//   8 | CreateAsyncQueryable<T>(Func<Expr,CT,ValueTask<DO>>, typeInfo, pred, mapper)    | Should_execute_typed_dynamic_object_overloads
//   9 | CreateAsyncQueryable(Type, Func<Expr,ValueTask<object?>>, ctx = null, mapper…)  | Should_execute_untyped_object_overloads
//  10 | CreateAsyncQueryable(Type, Func<Expr,ValueTask<object?>>, typeInfo, pred, …)    | Should_execute_untyped_object_overloads
//  11 | CreateAsyncQueryable(Type, Func<Expr,CT,ValueTask<object?>>, ctx = null, …)     | Should_execute_untyped_object_overloads
//  12 | CreateAsyncQueryable(Type, Func<Expr,CT,ValueTask<object?>>, typeInfo, pred, …) | Should_execute_untyped_object_overloads
//  13 | CreateAsyncQueryable<T>(Func<Expr,ValueTask<object?>>, ctx = null, mapper = …)  | Should_execute_typed_object_overloads
//  14 | CreateAsyncQueryable<T>(Func<Expr,ValueTask<object?>>, typeInfo, pred = null,…) | Should_execute_typed_object_overloads
//  15 | CreateAsyncQueryable<T>(Func<Expr,CT,ValueTask<object?>>, ctx = null, mapper…)  | Should_execute_typed_object_overloads
//  16 | CreateAsyncQueryable<T>(Func<Expr,CT,ValueTask<object?>>, typeInfo, pred, …)    | Should_execute_typed_object_overloads
//  17 | CreateAsyncQueryable<TS>(Type, Func<Expr,ValueTask<TS?>>, mapper)               | Should_execute_custom_source_overloads
//  18 | CreateAsyncQueryable<TS>(Type, Func<Expr,ValueTask<TS?>>, ctx, mapper)          | Should_execute_custom_source_overloads
//  19 | CreateAsyncQueryable<TS>(Type, Func<Expr,ValueTask<TS?>>, typeInfo, mapper)     | Should_execute_custom_source_overloads
//  20 | CreateAsyncQueryable<TS>(Type, Func<Expr,ValueTask<TS?>>, typeInfo, pred, …)    | Should_execute_custom_source_overloads
//  21 | CreateAsyncQueryable<TS>(Type, Func<Expr,CT,ValueTask<TS?>>, mapper)            | Should_execute_custom_source_overloads
//  22 | CreateAsyncQueryable<TS>(Type, Func<Expr,CT,ValueTask<TS?>>, ctx, mapper)       | Should_execute_custom_source_overloads
//  23 | CreateAsyncQueryable<TS>(Type, Func<Expr,CT,ValueTask<TS?>>, typeInfo, mapper)  | Should_execute_custom_source_overloads
//  24 | CreateAsyncQueryable<TS>(Type, Func<Expr,CT,ValueTask<TS?>>, typeInfo, pred, …) | Should_execute_custom_source_overloads
//  25 | CreateAsyncQueryable<T,TS>(Func<Expr,ValueTask<TS?>>, ctx, mapper)              | Should_execute_typed_custom_source_overloads
//  26 | CreateAsyncQueryable<T,TS>(Func<Expr,ValueTask<TS?>>, typeInfo, mapper)         | Should_execute_typed_custom_source_overloads
//  27 | CreateAsyncQueryable<T,TS>(Func<Expr,ValueTask<TS?>>, typeInfo, pred, mapper)   | Should_execute_typed_custom_source_overloads
//  28 | CreateAsyncQueryable<T,TS>(Func<Expr,CT,ValueTask<TS?>>, mapper)                | Should_execute_typed_custom_source_overloads
//  29 | CreateAsyncQueryable<T,TS>(Func<Expr,CT,ValueTask<TS?>>, ctx, mapper)           | Should_execute_typed_custom_source_overloads
//  30 | CreateAsyncQueryable<T,TS>(Func<Expr,CT,ValueTask<TS?>>, typeInfo, mapper)      | Should_execute_typed_custom_source_overloads
//  31 | CreateAsyncQueryable<T,TS>(Func<Expr,CT,ValueTask<TS?>>, typeInfo, pred, mapper)| Should_execute_typed_custom_source_overloads
#nullable enable
namespace Remote.Linq.Tests.RemoteQueryableFactory;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.Async;
using Remote.Linq.DynamicQuery;
using RemoteLinq = Remote.Linq.Expressions;
using RemoteQueryable = Remote.Linq.RemoteQueryable;
using SystemLinq = System.Linq.Expressions;

public class When_using_async_factory_overloads
{
    private class ItemDto
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private class SourceItem
    {
        public int Count { get; set; }
    }

    /// <summary>
    /// Untyped <see cref="IAsyncRemoteQueryable"/> <see cref="DynamicObject"/> overloads (overloads 1-4).
    /// The async scalar operation maps the single <see cref="DynamicObject"/> record to <see cref="ItemDto"/>.
    /// </summary>
    [Fact]
    public async Task Should_execute_untyped_dynamic_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var descriptorType = typeof(ItemDto);
        var token = TestContext.Current.CancellationToken;

        // overload 1: untyped DynamicObject with optional context (default mapper)
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(42));
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var queryable = factory.CreateAsyncQueryable(descriptorType, dataProvider);
            var result = await queryable.Provider.ExecuteAsync<ItemDto>(queryable.Expression, token);
            result.Count.ShouldBe(42);
            provider.InvocationCount.ShouldBe(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 2: untyped DynamicObject with type info provider and local-evaluation predicate
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(43));
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var queryable = factory.CreateAsyncQueryable(descriptorType, dataProvider, typeInfoProvider, predicate.Predicate);
            var result = await queryable.Provider.ExecuteAsync<ItemDto>(queryable.Expression, token);
            result.Count.ShouldBe(43);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 3: untyped DynamicObject with cancellation token and explicit context
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(44));
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncQueryable(descriptorType, provider.DataProvider, context);
            var result = await queryable.Provider.ExecuteAsync<ItemDto>(queryable.Expression, token);
            result.Count.ShouldBe(44);
            provider.InvocationCount.ShouldBe(1);
            provider.LastToken.ShouldBe(token);
            context.NeedsMapping(new object()).ShouldBeFalse();
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 4: untyped DynamicObject with cancellation token, type info provider, predicate, and explicit mapper
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(45));
            var mapper = new AsyncDynamicResultMapper();
            var queryable = factory.CreateAsyncQueryable(descriptorType, provider.DataProvider, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, mapper);
            var result = await queryable.Provider.ExecuteAsync<ItemDto>(queryable.Expression, token);
            result.Count.ShouldBe(45);
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }
    }

    /// <summary>
    /// Typed <see cref="IAsyncRemoteQueryable{T}"/> <see cref="DynamicObject"/> overloads (overloads 5-8).
    /// The async scalar operation uses <c>FirstAsync</c>.
    /// </summary>
    [Fact]
    public async Task Should_execute_typed_dynamic_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 5: typed DynamicObject with optional context (default mapper)
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(51));
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var queryable = factory.CreateAsyncQueryable<ItemDto>(dataProvider);
            var result = await queryable.FirstAsync(token);
            result.Count.ShouldBe(51);
            provider.InvocationCount.ShouldBe(1);
        }

        // overload 6: typed DynamicObject with type info provider and local-evaluation predicate
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(52));
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var queryable = factory.CreateAsyncQueryable<ItemDto>(dataProvider, typeInfoProvider, predicate.Predicate);
            var result = await queryable.FirstAsync(token);
            result.Count.ShouldBe(52);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 7: typed DynamicObject with cancellation token and explicit context
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(53));
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncQueryable<ItemDto>(provider.DataProvider, context);
            var result = await queryable.FirstAsync(token);
            result.Count.ShouldBe(53);
            provider.LastToken.ShouldBe(token);
            context.ExpressionTranslator.ShouldNotBeNull();
        }

        // overload 8: typed DynamicObject with cancellation token, type info provider, predicate, and explicit mapper
        {
            var provider = new RecordingAsyncProvider<DynamicObject>(CreateDynamicObject(54));
            var mapper = new AsyncDynamicResultMapper();
            var queryable = factory.CreateAsyncQueryable<ItemDto>(provider.DataProvider, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, mapper);
            var result = await queryable.FirstAsync(token);
            result.Count.ShouldBe(54);
            provider.LastToken.ShouldBe(token);
        }
    }

    /// <summary>
    /// Untyped <see cref="IAsyncRemoteQueryable"/> <see cref="object"/> overloads (overloads 9-12).
    /// The async sequence operation maps the returned <see cref="SourceItem"/> array to <see cref="IEnumerable{T}"/>.
    /// </summary>
    [Fact]
    public async Task Should_execute_untyped_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var descriptorType = typeof(ItemDto);
        var token = TestContext.Current.CancellationToken;

        // overload 9: untyped object with optional context (default mapper)
        {
            var item = new SourceItem { Count = 9 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var queryable = factory.CreateAsyncQueryable(descriptorType, dataProvider);
            var items = await queryable.Provider.ExecuteAsync<IEnumerable<SourceItem>>(queryable.Expression, token);
            items.Single().ShouldBeSameAs(item);
            provider.InvocationCount.ShouldBe(1);
            queryable.ResourceType.ShouldBe(descriptorType);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 10: untyped object with type info provider and local-evaluation predicate
        {
            var item = new SourceItem { Count = 10 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var queryable = factory.CreateAsyncQueryable(descriptorType, dataProvider, typeInfoProvider, predicate.Predicate);
            var items = await queryable.Provider.ExecuteAsync<IEnumerable<SourceItem>>(queryable.Expression, token);
            items.Single().ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 11: untyped object with cancellation token and explicit context
        {
            var item = new SourceItem { Count = 11 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncQueryable(descriptorType, provider.DataProvider, context);
            var items = await queryable.Provider.ExecuteAsync<IEnumerable<SourceItem>>(queryable.Expression, token);
            items.Single().ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 12: untyped object with cancellation token, type info provider, predicate, and explicit mapper
        {
            var item = new SourceItem { Count = 12 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncQueryable(descriptorType, provider.DataProvider, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, mapper);
            var items = await queryable.Provider.ExecuteAsync<IEnumerable<SourceItem>>(queryable.Expression, token);
            items.Single().ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            mapper.InvocationCount.ShouldBe(1);
            mapper.LastToken.ShouldBe(token);
        }
    }

    /// <summary>
    /// Typed <see cref="IAsyncRemoteQueryable{T}"/> <see cref="object"/> overloads (overloads 13-16).
    /// Executes one async sequence operation and one async scalar operation per overload.
    /// </summary>
    [Fact]
    public async Task Should_execute_typed_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 13: typed object with optional context (default mapper)
        {
            var item = new SourceItem { Count = 13 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var queryable = factory.CreateAsyncQueryable<SourceItem>(dataProvider);
            var items = await queryable.ExecuteAsync<SourceItem>(token);
            items.Single().ShouldBeSameAs(item);
            var first = await queryable.FirstAsync(token);
            first.ShouldBeSameAs(item);
            provider.InvocationCount.ShouldBe(2);
        }

        // overload 14: typed object with type info provider and local-evaluation predicate
        {
            var item = new SourceItem { Count = 14 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(dataProvider, typeInfoProvider, predicate.Predicate);
            var items = await queryable.ExecuteAsync<SourceItem>(token);
            items.Single().ShouldBeSameAs(item);
            var first = await queryable.FirstAsync(token);
            first.ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 15: typed object with cancellation token and explicit context
        {
            var item = new SourceItem { Count = 15 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(provider.DataProvider, context);
            var items = await queryable.ExecuteAsync<SourceItem>(token);
            items.Single().ShouldBeSameAs(item);
            var first = await queryable.FirstAsync(token);
            first.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
        }

        // overload 16: typed object with cancellation token, type info provider, predicate, and explicit mapper
        {
            var item = new SourceItem { Count = 16 };
            var provider = new RecordingAsyncProvider<object?>(new[] { item });
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(provider.DataProvider, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, mapper);
            var items = await queryable.ExecuteAsync<SourceItem>(token);
            items.Single().ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            mapper.InvocationCount.ShouldBe(1);
            mapper.LastToken.ShouldBe(token);
        }
    }

    /// <summary>
    /// Untyped <see cref="IAsyncRemoteQueryable"/> custom-source overloads <c>CreateAsyncQueryable&lt;TSource&gt;</c> (overloads 17-24).
    /// The async scalar operation maps the source item through the explicit result mapper.
    /// </summary>
    [Fact]
    public async Task Should_execute_custom_source_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var descriptorType = typeof(ItemDto);
        var token = TestContext.Current.CancellationToken;

        // overload 17: untyped custom source with default (null) context
        {
            var item = new SourceItem { Count = 17 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, dataProvider, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            mapper.InvocationCount.ShouldBe(1);
            mapper.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 18: untyped custom source with explicit context
        {
            var item = new SourceItem { Count = 18 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var context = new RecordingTranslationContext();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, dataProvider, context, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            context.NeedsMapping(new object()).ShouldBeFalse();
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 19: untyped custom source with type info provider
        {
            var item = new SourceItem { Count = 19 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, dataProvider, typeInfoProvider, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 20: untyped custom source with type info provider and local-evaluation predicate
        {
            var item = new SourceItem { Count = 20 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, dataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 21: untyped custom source with cancellation token and default (null) context
        {
            var item = new SourceItem { Count = 21 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, provider.DataProvider, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            mapper.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 22: untyped custom source with cancellation token and explicit context
        {
            var item = new SourceItem { Count = 22 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var context = new RecordingTranslationContext();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, provider.DataProvider, context, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 23: untyped custom source with cancellation token and type info provider
        {
            var item = new SourceItem { Count = 23 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, provider.DataProvider, typeInfoProvider, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }

        // overload 24: untyped custom source with cancellation token, type info provider, and local-evaluation predicate
        {
            var item = new SourceItem { Count = 24 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<SourceItem>(descriptorType, provider.DataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var result = await queryable.Provider.ExecuteAsync<SourceItem>(queryable.Expression, token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            AssertContainsResourceDescriptor(provider.LastExpression, descriptorType);
        }
    }

    /// <summary>
    /// Typed <see cref="IAsyncRemoteQueryable{T}"/> custom-source overloads <c>CreateAsyncQueryable&lt;T,TSource&gt;</c> (overloads 25-31).
    /// The async scalar operation executes through the query provider with result type <see cref="SourceItem"/>.
    /// </summary>
    [Fact]
    public async Task Should_execute_typed_custom_source_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 25: typed custom source with explicit context
        {
            var item = new SourceItem { Count = 25 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(dataProvider, new RecordingTranslationContext(), mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            mapper.InvocationCount.ShouldBe(1);
            mapper.LastToken.ShouldBe(token);
        }

        // overload 26: typed custom source with type info provider
        {
            var item = new SourceItem { Count = 26 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(dataProvider, typeInfoProvider, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 27: typed custom source with type info provider and local-evaluation predicate
        {
            var item = new SourceItem { Count = 27 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = expression => provider.DataProvider(expression, CancellationToken.None);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(dataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 28: typed custom source with cancellation token and default (null) context
        {
            var item = new SourceItem { Count = 28 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(provider.DataProvider, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            mapper.LastToken.ShouldBe(token);
        }

        // overload 29: typed custom source with cancellation token and explicit context
        {
            var item = new SourceItem { Count = 29 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var context = new RecordingTranslationContext();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(provider.DataProvider, context, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            context.ExpressionTranslator.ShouldNotBeNull();
        }

        // overload 30: typed custom source with cancellation token and type info provider
        {
            var item = new SourceItem { Count = 30 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(provider.DataProvider, typeInfoProvider, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 31: typed custom source with cancellation token, type info provider, and local-evaluation predicate
        {
            var item = new SourceItem { Count = 31 };
            var provider = new RecordingAsyncProvider<SourceItem?>(item);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncQueryable<ItemDto, SourceItem>(provider.DataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var result = await queryable.ExecuteAsync<SourceItem>(token);
            result.ShouldBeSameAs(item);
            provider.LastToken.ShouldBe(token);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }
    }

    private static DynamicObject CreateDynamicObject(int count)
        => new(new[] { new Property("Count", count) });

    private static void AssertContainsResourceDescriptor(RemoteLinq.Expression? remoteExpression, Type elementType)
    {
        var constant = remoteExpression.ShouldNotBeNull().ShouldBeOfType<RemoteLinq.ConstantExpression>();
        constant.Value
            .ShouldBeOfType<QueryableResourceDescriptor>()
            .Type
            .ToType()
            .ShouldBe(elementType);
    }
}
