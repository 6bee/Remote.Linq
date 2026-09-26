// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq;
using Remote.Linq.Async;
using Remote.Linq.DynamicQuery;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

// Overload call table (one row per declaration; implementation checklist, not a coverage-analysis artifact):
//
//   S1   CreateEntityFrameworkCoreQueryable(Type, Func<RemoteLinq.Expression, DynamicObject>, IExpressionToRemoteLinqContext, IQueryResultMapper<DynamicObject>)
//        -> Should_execute_sync_dynamic_object_overloads
//   S2   CreateEntityFrameworkCoreQueryable(Type, Func<RemoteLinq.Expression, DynamicObject>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<DynamicObject>)
//        -> Should_execute_sync_dynamic_object_overloads
//   S3   CreateEntityFrameworkCoreQueryable<T>(Func<RemoteLinq.Expression, DynamicObject>, IExpressionToRemoteLinqContext, IQueryResultMapper<DynamicObject>)
//        -> Should_execute_sync_dynamic_object_overloads
//   S4   CreateEntityFrameworkCoreQueryable<T>(Func<RemoteLinq.Expression, DynamicObject>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<DynamicObject>)
//        -> Should_execute_sync_dynamic_object_overloads
//   S5   CreateEntityFrameworkCoreQueryable(Type, Func<RemoteLinq.Expression, object>, IExpressionToRemoteLinqContext, IQueryResultMapper<object>)
//        -> Should_execute_sync_object_overloads
//   S6   CreateEntityFrameworkCoreQueryable(Type, Func<RemoteLinq.Expression, object>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<object>)
//        -> Should_execute_sync_object_overloads
//   S7   CreateEntityFrameworkCoreQueryable<T>(Func<RemoteLinq.Expression, object>, IExpressionToRemoteLinqContext, IQueryResultMapper<object>)
//        -> Should_execute_sync_object_overloads
//   S8   CreateEntityFrameworkCoreQueryable<T>(Func<RemoteLinq.Expression, object>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<object>)
//        -> Should_execute_sync_object_overloads
//   S9   CreateEntityFrameworkCoreQueryable<TSource>(Type, Func<RemoteLinq.Expression, TSource>, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   S10  CreateEntityFrameworkCoreQueryable<TSource>(Type, Func<RemoteLinq.Expression, TSource>, IExpressionToRemoteLinqContext, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   S11  CreateEntityFrameworkCoreQueryable<TSource>(Type, Func<RemoteLinq.Expression, TSource>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   S12  CreateEntityFrameworkCoreQueryable<T, TSource>(Func<RemoteLinq.Expression, TSource>, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   S13  CreateEntityFrameworkCoreQueryable<T, TSource>(Func<RemoteLinq.Expression, TSource>, IExpressionToRemoteLinqContext, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   S14  CreateEntityFrameworkCoreQueryable<T, TSource>(Func<RemoteLinq.Expression, TSource>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IQueryResultMapper<TSource>)
//        -> Should_execute_sync_custom_source_overloads
//   A1   CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_with_token
//   A2   CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, ValueTask<DynamicObject>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_without_token
//   A3   CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_with_token
//   A4   CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, ValueTask<DynamicObject>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_without_token
//   A5   CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_with_token
//   A6   CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, ValueTask<DynamicObject>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_without_token
//   A7   CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, ValueTask<DynamicObject>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_without_token
//   A8   CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_dynamic_object_overloads_with_token
//   A9   CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_with_token
//   A10  CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, ValueTask<object>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_without_token
//   A11  CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_with_token
//   A12  CreateEntityFrameworkCoreAsyncQueryable(Type, Func<RemoteLinq.Expression, ValueTask<object>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_without_token
//   A13  CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_with_token
//   A14  CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, ValueTask<object>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_without_token
//   A15  CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, ValueTask<object>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_without_token
//   A16  CreateEntityFrameworkCoreAsyncQueryable<T>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_object_overloads_with_token
//   A17  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   A18  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, ValueTask<TSource>>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A19  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   A20  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, ValueTask<TSource>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A21  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, ValueTask<TSource>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A22  CreateEntityFrameworkCoreAsyncQueryable<TSource>(Type, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   A23  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, ValueTask<TSource>>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A24  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   A25  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, ValueTask<TSource>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A26  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   A27  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, ValueTask<TSource>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_without_token
//   A28  CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_async_custom_source_overloads_with_token
//   ST1  CreateEntityFrameworkCoreAsyncStreamQueryable<T>(Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_stream_overloads
//   ST2  CreateEntityFrameworkCoreAsyncStreamQueryable<T>(Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<DynamicObject>)
//        -> Should_execute_async_stream_overloads
//   ST3  CreateEntityFrameworkCoreAsyncStreamQueryable<T>(Func<RemoteLinq.Expression, IAsyncEnumerable<object>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_stream_overloads
//   ST4  CreateEntityFrameworkCoreAsyncStreamQueryable<T>(Func<RemoteLinq.Expression, IAsyncEnumerable<object>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<object>)
//        -> Should_execute_async_stream_overloads
//   ST5  CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_explicit_result_mapper_overloads
//   ST6  CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>>, IExpressionToRemoteLinqContext, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_explicit_result_mapper_overloads
//   ST7  CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>>, ITypeInfoProvider, Func<SystemLinq.Expression, bool>, IAsyncQueryResultMapper<TSource>)
//        -> Should_execute_explicit_result_mapper_overloads
public sealed class When_using_entity_framework_core_factory_overloads : IDisposable
{
    private readonly TestDbContext _context = new();

    public When_using_entity_framework_core_factory_overloads()
    {
        _context.Lookup.AddRange(
            new LookupItem { Key = "1", Value = "One" },
            new LookupItem { Key = "2", Value = "Two" },
            new LookupItem { Key = "3", Value = "Three" });
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void Should_execute_sync_dynamic_object_overloads()
    {
        Func<RemoteLinq.Expression, DynamicObject> dataProvider =
            expression => expression.ExecuteWithEntityFrameworkCore(_context);

        // S1: untyped DynamicObject with default context and default mapper.
        IRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable(typeof(LookupItem), dataProvider, context: null);
        var untypedItems = untypedQueryable.Provider.Execute<LookupItem[]>(untypedQueryable.Expression);
        untypedItems.Length.ShouldBe(3);
        untypedItems[0].Key.ShouldBe("1");

        // S2: untyped DynamicObject with type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = untypedQueryable2.Provider.Execute<LookupItem[]>(untypedQueryable2.Expression);
        untypedItems2.Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // S3: typed DynamicObject with default context and default mapper.
        IRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(dataProvider, context: null);
        var typedItems = typedQueryable.Execute().ToArray();
        typedItems[1].Key.ShouldBe("2");

        // S4: typed DynamicObject with type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        typedQueryable2.Execute().Single(x => x.Key == "3").Value.ShouldBe("Three");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Should_execute_sync_object_overloads()
    {
        Func<RemoteLinq.Expression, object> dataProvider = _ => (object)_context.Lookup;

        // S5: untyped object with default context and default mapper.
        IRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable(typeof(LookupItem), dataProvider, context: null);
        var untypedItems = untypedQueryable.Provider.Execute<IQueryable<LookupItem>>(untypedQueryable.Expression);
        untypedItems.Count().ShouldBe(3);

        // S6: untyped object with type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = untypedQueryable2.Provider.Execute<IQueryable<LookupItem>>(untypedQueryable2.Expression);
        untypedItems2.Single(x => x.Key == "1").Value.ShouldBe("One");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // S7: typed object with default context and default mapper.
        IRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(dataProvider, context: null);
        var typedItems = typedQueryable.Execute().ToArray();
        typedItems[2].Key.ShouldBe("3");

        // S8: typed object with type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        typedQueryable2.Execute().Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void Should_execute_sync_custom_source_overloads()
    {
        Func<RemoteLinq.Expression, IQueryable<LookupItem>> dataProvider = _ => _context.Lookup;
        var mapper = new CastResultMapper<IQueryable<LookupItem>>();
        var explicitContext = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => true);

        // S9: untyped custom source with required result mapper.
        IRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, mapper);
        var untypedItems = untypedQueryable.Provider.Execute<IQueryable<LookupItem>>(untypedQueryable.Expression);
        untypedItems.Count().ShouldBe(3);

        // S10: untyped custom source with explicit context and required result mapper.
        IRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, explicitContext, mapper);
        var untypedItems2 = untypedQueryable2.Provider.Execute<IQueryable<LookupItem>>(untypedQueryable2.Expression);
        untypedItems2.Single(x => x.Key == "1").Key.ShouldBe("1");

        // S11: untyped custom source with type info provider, local-evaluation predicate and required result mapper.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IRemoteQueryable untypedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<IQueryable<LookupItem>>(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        var untypedItems3 = untypedQueryable3.Provider.Execute<IQueryable<LookupItem>>(untypedQueryable3.Expression);
        untypedItems3.Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // S12: typed custom source with required result mapper.
        IRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, mapper);
        typedQueryable.Execute().Count().ShouldBe(3);

        // S13: typed custom source with explicit context and required result mapper.
        IRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, explicitContext, mapper);
        typedQueryable2.Execute().Single(x => x.Key == "3").Value.ShouldBe("Three");

        // S14: typed custom source with type info provider, local-evaluation predicate and required result mapper.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IRemoteQueryable<LookupItem> typedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem, IQueryable<LookupItem>>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        typedQueryable3.Execute().Single(x => x.Key == "1").Value.ShouldBe("One");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_execute_async_dynamic_object_overloads_with_token()
    {
        var token = TestContext.Current.CancellationToken;
        var forwardedTokens = new List<CancellationToken>();
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider =
            (expression, cancellationToken) =>
            {
                forwardedTokens.Add(cancellationToken);
                return expression.ExecuteWithEntityFrameworkCoreAsync(_context, cancellationToken);
            };

        // A1: untyped DynamicObject with token-aware provider, default context and default mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(typeof(LookupItem), dataProvider);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<LookupItem[]>(untypedQueryable.Expression, token);
        untypedItems.Length.ShouldBe(3);

        // A3: untyped DynamicObject with token-aware provider, type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<LookupItem[]>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A5: typed DynamicObject with token-aware provider, default context and default mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(dataProvider, context: null);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A8: typed DynamicObject with token-aware provider, type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "3").Value.ShouldBe("Three");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        forwardedTokens.Count.ShouldBe(4);
        forwardedTokens.All(x => x == token).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_execute_async_dynamic_object_overloads_without_token()
    {
        var token = TestContext.Current.CancellationToken;
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider =
            expression => expression.ExecuteWithEntityFrameworkCoreAsync(_context);

        // A2: untyped DynamicObject with non-token provider, default context and default mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(typeof(LookupItem), dataProvider);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<LookupItem[]>(untypedQueryable.Expression, token);
        untypedItems.Length.ShouldBe(3);

        // A4: untyped DynamicObject with non-token provider, type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<LookupItem[]>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "1").Value.ShouldBe("One");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A6: typed DynamicObject with non-token provider, default context and default mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(dataProvider);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A7: typed DynamicObject with non-token provider, type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "2").Key.ShouldBe("2");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_execute_async_object_overloads_with_token()
    {
        var token = TestContext.Current.CancellationToken;
        var forwardedTokens = new List<CancellationToken>();
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object>> dataProvider =
            (_, cancellationToken) =>
            {
                forwardedTokens.Add(cancellationToken);
                return ValueTask.FromResult<object>(_context.Lookup);
            };

        // A9: untyped object with token-aware provider, default context and default mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(typeof(LookupItem), dataProvider);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable.Expression, token);
        untypedItems.Count().ShouldBe(3);

        // A11: untyped object with token-aware provider, type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "1").Value.ShouldBe("One");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A13: typed object with token-aware provider, default context and default mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(dataProvider);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A16: typed object with token-aware provider, type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "3").Value.ShouldBe("Three");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        forwardedTokens.Count.ShouldBe(4);
        forwardedTokens.All(x => x == token).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_execute_async_object_overloads_without_token()
    {
        var token = TestContext.Current.CancellationToken;
        Func<RemoteLinq.Expression, ValueTask<object>> dataProvider =
            _ => ValueTask.FromResult<object>(_context.Lookup);

        // A10: untyped object with non-token provider, default context and default mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(typeof(LookupItem), dataProvider);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable.Expression, token);
        untypedItems.Count().ShouldBe(3);

        // A12: untyped object with non-token provider, type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A14: typed object with non-token provider, default context and default mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(dataProvider);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A15: typed object with non-token provider, type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "1").Key.ShouldBe("1");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_execute_async_custom_source_overloads_with_token()
    {
        var token = TestContext.Current.CancellationToken;
        var forwardedTokens = new List<CancellationToken>();
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<IQueryable<LookupItem>>> dataProvider =
            (_, cancellationToken) =>
            {
                forwardedTokens.Add(cancellationToken);
                return ValueTask.FromResult<IQueryable<LookupItem>>(_context.Lookup);
            };

        var mapper = new CastAsyncResultMapper<IQueryable<LookupItem>>();
        var explicitContext = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => true);
        var typeInfoProvider = new RecordingTypeInfoProvider();

        // A17: untyped custom source with token-aware provider and required result mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, mapper);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable.Expression, token);
        untypedItems.Count().ShouldBe(3);

        // A19: untyped custom source with token-aware provider, explicit context and required result mapper.
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, explicitContext, mapper);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "1").Value.ShouldBe("One");

        // A22: untyped custom source with token-aware provider, type info provider, local-evaluation predicate and required result mapper.
        IAsyncRemoteQueryable untypedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        var untypedItems3 = await untypedQueryable3.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable3.Expression, token);
        untypedItems3.Single(x => x.Key == "2").Key.ShouldBe("2");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A24: typed custom source with token-aware provider and required result mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, mapper);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A26: typed custom source with token-aware provider, explicit context and required result mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, explicitContext, mapper);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "3").Value.ShouldBe("Three");

        // A28: typed custom source with token-aware provider, type info provider, local-evaluation predicate and required result mapper.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        (await typedQueryable3.ExecuteAsync(token)).Single(x => x.Key == "1").Key.ShouldBe("1");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        forwardedTokens.Count.ShouldBe(6);
        forwardedTokens.All(x => x == token).ShouldBeTrue();
    }

    [Fact]
    public async Task Should_execute_async_custom_source_overloads_without_token()
    {
        var token = TestContext.Current.CancellationToken;
        Func<RemoteLinq.Expression, ValueTask<IQueryable<LookupItem>>> dataProvider =
            _ => ValueTask.FromResult<IQueryable<LookupItem>>(_context.Lookup);

        var mapper = new CastAsyncResultMapper<IQueryable<LookupItem>>();
        var explicitContext = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => true);
        var typeInfoProvider = new RecordingTypeInfoProvider();

        // A18: untyped custom source with non-token provider and required result mapper.
        IAsyncRemoteQueryable untypedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, mapper);
        var untypedItems = await untypedQueryable.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable.Expression, token);
        untypedItems.Count().ShouldBe(3);

        // A20: untyped custom source with non-token provider, explicit context and required result mapper.
        IAsyncRemoteQueryable untypedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(typeof(LookupItem), dataProvider, explicitContext, mapper);
        var untypedItems2 = await untypedQueryable2.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable2.Expression, token);
        untypedItems2.Single(x => x.Key == "1").Key.ShouldBe("1");

        // A21: untyped custom source with non-token provider, type info provider, local-evaluation predicate and required result mapper.
        IAsyncRemoteQueryable untypedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<IQueryable<LookupItem>>(
            typeof(LookupItem),
            dataProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        var untypedItems3 = await untypedQueryable3.Provider.ExecuteAsync<IQueryable<LookupItem>>(untypedQueryable3.Expression, token);
        untypedItems3.Single(x => x.Key == "2").Value.ShouldBe("Two");
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // A23: typed custom source with non-token provider and required result mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, mapper);
        (await typedQueryable.ExecuteAsync(token)).Count().ShouldBe(3);

        // A25: typed custom source with non-token provider, explicit context and required result mapper.
        IAsyncRemoteQueryable<LookupItem> typedQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(dataProvider, explicitContext, mapper);
        (await typedQueryable2.ExecuteAsync(token)).Single(x => x.Key == "3").Value.ShouldBe("Three");

        // A27: typed custom source with non-token provider, type info provider, local-evaluation predicate and required result mapper.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteQueryable<LookupItem> typedQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem, IQueryable<LookupItem>>(
            dataProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null,
            mapper);
        (await typedQueryable3.ExecuteAsync(token)).Single(x => x.Key == "1").Key.ShouldBe("1");
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_execute_async_stream_overloads()
    {
        var token = TestContext.Current.CancellationToken;
        Func<Type, IQueryable> queryableProvider = type => type == typeof(LookupItem)
            ? _context.Lookup
            : throw new NotSupportedException();

        // ST1: typed DynamicObject stream with default context and default mapper.
        Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dynamicObjectProvider =
            expression => expression.ExecuteAsyncStreamWithEntityFrameworkCore(queryableProvider);
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem>(dynamicObjectProvider, context: null);
        var streamItems = await ToArrayAsync(streamQueryable.Where(x => x.Key == "1").AsAsyncEnumerable(cancellation: token));
        streamItems.Single().Key.ShouldBe("1");

        // ST2: typed DynamicObject stream with type info provider and local-evaluation predicate.
        var typeInfoProvider = new RecordingTypeInfoProvider();
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem>(
            dynamicObjectProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null);
        var streamItems2 = await ToArrayAsync(streamQueryable2.Where(x => x.Value.ToUpper().Contains("O")).AsAsyncEnumerable(cancellation: token));
        streamItems2.Length.ShouldBe(2);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);

        // ST3: typed object stream with default context and default mapper.
        Func<RemoteLinq.Expression, IAsyncEnumerable<object>> objectProvider =
            expression => expression.ExecuteAsyncStreamWithEntityFrameworkCore(_context);
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem>(objectProvider, context: null);
        var streamItems3 = await ToArrayAsync(streamQueryable3.Where(x => x.Key == "2").AsAsyncEnumerable(cancellation: token));
        streamItems3.Single().Value.ShouldBe("Two");

        // ST4: typed object stream with type info provider and local-evaluation predicate.
        var typeInfoProvider2 = new RecordingTypeInfoProvider();
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable4 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem>(
            objectProvider,
            typeInfoProvider2,
            (Func<SystemLinq.Expression, bool>)null);
        var streamItems4 = await ToArrayAsync(streamQueryable4.Where(x => x.Value.ToUpper().Contains("E")).AsAsyncEnumerable(cancellation: token));
        streamItems4.Length.ShouldBe(2);
        typeInfoProvider2.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Should_execute_explicit_result_mapper_overloads()
    {
        var token = TestContext.Current.CancellationToken;
        Func<RemoteLinq.Expression, IAsyncEnumerable<object>> objectProvider =
            expression => expression.ExecuteAsyncStreamWithEntityFrameworkCore(_context);

        var explicitContext = new EntityFrameworkCoreExpressionTranslatorContext((ITypeInfoProvider)null, _ => true);
        var typeInfoProvider = new RecordingTypeInfoProvider();

        // ST5: custom-source stream with required result mapper.
        var mapper = new CollectingStreamResultMapper();
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem, object>(objectProvider, mapper);
        var streamItems = await ToArrayAsync(streamQueryable.Where(x => x.Key == "1").AsAsyncEnumerable(cancellation: token));
        streamItems.Single().Value.ShouldBe("One");
        mapper.InvocationCount.ShouldBe(1);

        // ST6: custom-source stream with explicit context and required result mapper.
        var mapper2 = new CollectingStreamResultMapper();
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable2 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem, object>(objectProvider, explicitContext, mapper2);
        var streamItems2 = await ToArrayAsync(streamQueryable2.Where(x => x.Key == "2").AsAsyncEnumerable(cancellation: token));
        streamItems2.Single().Key.ShouldBe("2");
        mapper2.InvocationCount.ShouldBe(1);

        // ST7: custom-source stream with type info provider, local-evaluation predicate and required result mapper.
        var mapper3 = new CollectingStreamResultMapper();
        IAsyncRemoteStreamQueryable<LookupItem> streamQueryable3 = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem, object>(
            objectProvider,
            typeInfoProvider,
            (Func<SystemLinq.Expression, bool>)null,
            mapper3);
        var streamItems3 = await ToArrayAsync(streamQueryable3.AsAsyncEnumerable(cancellation: token));
        streamItems3.Length.ShouldBe(3);
        mapper3.InvocationCount.ShouldBe(3);
        typeInfoProvider.InvocationCount.ShouldBeGreaterThanOrEqualTo(1);
    }

    private static async Task<T[]> ToArrayAsync<T>(IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
        {
            list.Add(item);
        }

        return list.ToArray();
    }

    private sealed class RecordingTypeInfoProvider : ITypeInfoProvider
    {
        private int _invocationCount;

        public int InvocationCount => _invocationCount;

        public TypeInfo GetTypeInfo(Type type, bool? includePropertyInfos, bool? setMemberDeclaringTypes)
        {
            Interlocked.Increment(ref _invocationCount);
            return type?.AsTypeInfo();
        }
    }

    private sealed class CastResultMapper<TSource> : IQueryResultMapper<TSource>
    {
        public TResult MapResult<TResult>(TSource source, SystemLinq.Expression expression)
            => (TResult)(object)source;
    }

    private sealed class CastAsyncResultMapper<TSource> : IAsyncQueryResultMapper<TSource>
    {
        public ValueTask<TResult> MapResultAsync<TResult>(TSource source, SystemLinq.Expression expression, CancellationToken cancellationToken)
            => new((TResult)(object)source);
    }

    private sealed class CollectingStreamResultMapper : IAsyncQueryResultMapper<object>
    {
        private int _invocationCount;

        public int InvocationCount => _invocationCount;

        public ValueTask<TResult> MapResultAsync<TResult>(object source, SystemLinq.Expression expression, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _invocationCount);
            return new ValueTask<TResult>((TResult)source);
        }
    }
}
