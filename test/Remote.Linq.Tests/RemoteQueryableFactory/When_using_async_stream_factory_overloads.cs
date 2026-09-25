// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

// Overload-call table for the 18 public CreateAsyncStreamQueryable overloads
// in src/Remote.Linq/RemoteQueryableFactoryAsyncStreamExtensions.cs:
//
//  #  | Overload (provider result / token shape / context shape)                        | Invoked by
//  --- | ------------------------------------------------------------------------------ | ----------------------------------
//   1 | CreateAsyncStreamQueryable<T>(Func<Expr,IAE<DO>>, ctx = null, mapper = null)    | Should_execute_typed_dynamic_object_overloads
//   2 | CreateAsyncStreamQueryable<T>(Func<Expr,IAE<DO>>, typeInfo, pred = null, …)     | Should_execute_typed_dynamic_object_overloads
//   3 | CreateAsyncStreamQueryable<T>(Func<Expr,CT,IAE<DO>>, ctx = null, mapper = null) | Should_execute_typed_dynamic_object_overloads
//   4 | CreateAsyncStreamQueryable<T>(Func<Expr,CT,IAE<DO>>, typeInfo, pred, mapper)    | Should_execute_typed_dynamic_object_overloads
//   5 | CreateAsyncStreamQueryable<T>(Func<Expr,IAE<object?>>, ctx = null, mapper = …)  | Should_execute_typed_object_overloads
//   6 | CreateAsyncStreamQueryable<T>(Func<Expr,IAE<object?>>, typeInfo, mapper)        | Should_execute_typed_object_overloads
//   7 | CreateAsyncStreamQueryable<T>(Func<Expr,IAE<object?>>, typeInfo, pred, mapper)  | Should_execute_typed_object_overloads
//   8 | CreateAsyncStreamQueryable<T>(Func<Expr,CT,IAE<object?>>, ctx = null, mapper…)  | Should_execute_typed_object_overloads
//   9 | CreateAsyncStreamQueryable<T>(Func<Expr,CT,IAE<object?>>, typeInfo, mapper)     | Should_execute_typed_object_overloads
//  10 | CreateAsyncStreamQueryable<T>(Func<Expr,CT,IAE<object?>>, typeInfo, pred, …)    | Should_execute_typed_object_overloads
//  11 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,IAE<TS?>>, mapper)                   | Should_execute_custom_source_overloads
//  12 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,IAE<TS?>>, ctx, mapper)              | Should_execute_custom_source_overloads
//  13 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,IAE<TS?>>, typeInfo, mapper)         | Should_execute_custom_source_overloads
//  14 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,IAE<TS?>>, typeInfo, pred, mapper)   | Should_execute_custom_source_overloads
//  15 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,CT,IAE<TS?>>, mapper)                | Should_execute_custom_source_overloads
//  16 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,CT,IAE<TS?>>, ctx, mapper)           | Should_execute_custom_source_overloads
//  17 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,CT,IAE<TS?>>, typeInfo, mapper)      | Should_execute_custom_source_overloads
//  18 | CreateAsyncStreamQueryable<T,TS>(Func<Expr,CT,IAE<TS?>>, typeInfo, pred, mapper)| Should_execute_custom_source_overloads
#nullable enable
namespace Remote.Linq.Tests.RemoteQueryableFactory;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.Async;
using Remote.Linq.DynamicQuery;
using RemoteLinq = Remote.Linq.Expressions;
using RemoteQueryable = Remote.Linq.RemoteQueryable;

public class When_using_async_stream_factory_overloads
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
    /// Typed <see cref="IAsyncRemoteStreamQueryable{T}"/> <see cref="DynamicObject"/> overloads (overloads 1-4).
    /// The async stream operation maps each <see cref="DynamicObject"/> record to <see cref="ItemDto"/>.
    /// </summary>
    [Fact]
    public async Task Should_execute_typed_dynamic_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 1: typed DynamicObject with optional context (default mapper)
        {
            var provider = new RecordingAsyncStreamProvider<DynamicObject>([CreateDynamicObject(1), CreateDynamicObject(2)]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider = provider.DataProvider;
            var queryable = factory.CreateAsyncStreamQueryable<ItemDto>(dataProvider);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items.Count.ShouldBe(2);
            items[0].Count.ShouldBe(1);
            items[1].Count.ShouldBe(2);
            provider.InvocationCount.ShouldBe(1);

            // the framework re-injects the execution token into the non-token provider via WithCancellation
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, typeof(ItemDto));
        }

        // overload 2: typed DynamicObject with type info provider and local-evaluation predicate
        {
            var provider = new RecordingAsyncStreamProvider<DynamicObject>([CreateDynamicObject(3), CreateDynamicObject(4)]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider = provider.DataProvider;
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var queryable = factory.CreateAsyncStreamQueryable<ItemDto>(dataProvider, typeInfoProvider, predicate.Predicate);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items.Select(x => x.Count).ShouldBe([3, 4]);
            provider.InvocationCount.ShouldBe(1);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 3: typed DynamicObject with cancellation token and explicit context
        {
            var provider = new RecordingAsyncStreamProvider<DynamicObject>([CreateDynamicObject(5), CreateDynamicObject(6)]);
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncStreamQueryable<ItemDto>(provider.DataProviderWithToken, context);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items.Select(x => x.Count).ShouldBe([5, 6]);
            provider.LastToken.ShouldBe(token);
            context.NeedsMapping(new object()).ShouldBeFalse();
        }

        // overload 4: typed DynamicObject with cancellation token, type info provider, predicate, and explicit mapper
        {
            var provider = new RecordingAsyncStreamProvider<DynamicObject>([CreateDynamicObject(7), CreateDynamicObject(8)]);
            var mapper = new AsyncDynamicStreamResultMapper();
            var queryable = factory.CreateAsyncStreamQueryable<ItemDto>(provider.DataProviderWithToken, new RecordingTypeInfoProvider(), new RecordingPredicate().Predicate, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items.Select(x => x.Count).ShouldBe([7, 8]);
            provider.LastToken.ShouldBe(token);
        }
    }

    /// <summary>
    /// Typed <see cref="IAsyncRemoteStreamQueryable{T}"/> <see cref="object"/> overloads (overloads 5-10).
    /// The async stream operation casts each <see cref="SourceItem"/> record to the result type.
    /// </summary>
    [Fact]
    public async Task Should_execute_typed_object_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 5: typed object with optional context (default caster)
        {
            var item1 = new SourceItem { Count = 5 };
            var item2 = new SourceItem { Count = 6 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider = provider.DataProvider;
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(dataProvider);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items.Count.ShouldBe(2);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.InvocationCount.ShouldBe(1);

            // the framework re-injects the execution token into the non-token provider via WithCancellation
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, typeof(SourceItem));
        }

        // overload 6: typed object with type info provider and explicit mapper
        {
            var item1 = new SourceItem { Count = 7 };
            var item2 = new SourceItem { Count = 8 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider = provider.DataProvider;
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(dataProvider, typeInfoProvider, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            mapper.InvocationCount.ShouldBe(2);
            mapper.LastToken.ShouldBe(token);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 7: typed object with type info provider, local-evaluation predicate, and explicit mapper
        {
            var item1 = new SourceItem { Count = 9 };
            var item2 = new SourceItem { Count = 10 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider = provider.DataProvider;
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(dataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            mapper.InvocationCount.ShouldBe(2);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 8: typed object with cancellation token and explicit context (default caster)
        {
            var item1 = new SourceItem { Count = 11 };
            var item2 = new SourceItem { Count = 12 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            var context = new RecordingTranslationContext();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(provider.DataProviderWithToken, context);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            context.NeedsMapping(new object()).ShouldBeFalse();
        }

        // overload 9: typed object with cancellation token, type info provider, and explicit mapper
        {
            var item1 = new SourceItem { Count = 13 };
            var item2 = new SourceItem { Count = 14 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(provider.DataProviderWithToken, typeInfoProvider, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            mapper.InvocationCount.ShouldBe(2);
            mapper.LastToken.ShouldBe(token);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 10: typed object with cancellation token, type info provider, local-evaluation predicate, and explicit mapper
        {
            var item1 = new SourceItem { Count = 15 };
            var item2 = new SourceItem { Count = 16 };
            var provider = new RecordingAsyncStreamProvider<object?>([item1, item2]);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<object>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem>(provider.DataProviderWithToken, typeInfoProvider, predicate.Predicate, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            mapper.InvocationCount.ShouldBe(2);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }
    }

    /// <summary>
    /// Typed <see cref="IAsyncRemoteStreamQueryable{T}"/> custom-source overloads <c>CreateAsyncStreamQueryable&lt;T,TSource&gt;</c> (overloads 11-18).
    /// The async stream operation passes each <see cref="SourceItem"/> through the explicit result mapper.
    /// </summary>
    [Fact]
    public async Task Should_execute_custom_source_overloads()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        // overload 11: custom source with default (null) context
        {
            var item1 = new SourceItem { Count = 11 };
            var item2 = new SourceItem { Count = 12 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<SourceItem>> dataProvider = provider.DataProvider;
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(dataProvider, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            mapper.InvocationCount.ShouldBe(2);
            mapper.LastToken.ShouldBe(token);
            provider.InvocationCount.ShouldBe(1);

            // the framework re-injects the execution token into the non-token provider via WithCancellation
            provider.LastToken.ShouldBe(token);
            AssertContainsResourceDescriptor(provider.LastExpression, typeof(SourceItem));
        }

        // overload 12: custom source with explicit context
        {
            var item1 = new SourceItem { Count = 13 };
            var item2 = new SourceItem { Count = 14 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<SourceItem>> dataProvider = provider.DataProvider;
            var context = new RecordingTranslationContext();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(dataProvider, context, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            context.NeedsMapping(new object()).ShouldBeFalse();
            mapper.InvocationCount.ShouldBe(2);
        }

        // overload 13: custom source with type info provider
        {
            var item1 = new SourceItem { Count = 15 };
            var item2 = new SourceItem { Count = 16 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<SourceItem>> dataProvider = provider.DataProvider;
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(dataProvider, typeInfoProvider, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            mapper.InvocationCount.ShouldBe(2);
        }

        // overload 14: custom source with type info provider and local-evaluation predicate
        {
            var item1 = new SourceItem { Count = 17 };
            var item2 = new SourceItem { Count = 18 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            Func<RemoteLinq.Expression, IAsyncEnumerable<SourceItem>> dataProvider = provider.DataProvider;
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(dataProvider, typeInfoProvider, predicate.Predicate, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 15: custom source with cancellation token and default (null) context
        {
            var item1 = new SourceItem { Count = 19 };
            var item2 = new SourceItem { Count = 20 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(provider.DataProviderWithToken, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            mapper.InvocationCount.ShouldBe(2);
            mapper.LastToken.ShouldBe(token);
        }

        // overload 16: custom source with cancellation token and explicit context
        {
            var item1 = new SourceItem { Count = 21 };
            var item2 = new SourceItem { Count = 22 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            var context = new RecordingTranslationContext();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(provider.DataProviderWithToken, context, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            context.ExpressionTranslator.ShouldNotBeNull();
        }

        // overload 17: custom source with cancellation token and type info provider
        {
            var item1 = new SourceItem { Count = 23 };
            var item2 = new SourceItem { Count = 24 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(provider.DataProviderWithToken, typeInfoProvider, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
        }

        // overload 18: custom source with cancellation token, type info provider, and local-evaluation predicate
        {
            var item1 = new SourceItem { Count = 25 };
            var item2 = new SourceItem { Count = 26 };
            var provider = new RecordingAsyncStreamProvider<SourceItem>([item1, item2]);
            var typeInfoProvider = new RecordingTypeInfoProvider();
            var predicate = new RecordingPredicate();
            var mapper = new PassthroughAsyncQueryResultMapper<SourceItem>();
            var queryable = factory.CreateAsyncStreamQueryable<SourceItem, SourceItem>(provider.DataProviderWithToken, typeInfoProvider, predicate.Predicate, mapper);
            var items = await CollectAsync(queryable.AsAsyncEnumerable(token), token);
            items[0].ShouldBeSameAs(item1);
            items[1].ShouldBeSameAs(item2);
            provider.LastToken.ShouldBe(token);
            predicate.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
            mapper.InvocationCount.ShouldBe(2);
        }
    }

    private static DynamicObject CreateDynamicObject(int count)
        => new(new[] { new Property("Count", count) });

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source, CancellationToken token)
    {
        var items = new List<T>();
        await foreach (var item in source.WithCancellation(token).ConfigureAwait(false))
        {
            items.Add(item);
        }

        return items;
    }

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
