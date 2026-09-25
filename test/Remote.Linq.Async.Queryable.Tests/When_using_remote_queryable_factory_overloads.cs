// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

// Overload-call table for the six public CreateAsyncQueryable overloads
// in src/Remote.Linq.Async.Queryable/RemoteQueryableFactoryExtensions.cs:
//
//  #  | Overload (delegate shape)                                                | Stream-only test (data provider null)   | Scalar-only test (stream provider null)
//  --- | ------------------------------------------------------------------------- | ---------------------------------------- | --------------------------------------
//   1 | CreateAsyncQueryable<T>(stream, data) [DynamicObject, no token]           | Should_execute_stream_with_dynamic_object_providers_without_token   | Should_execute_scalar_with_dynamic_object_providers_without_token
//   2 | CreateAsyncQueryable<T>(stream, data, mapper = null) [object, no token]   | Should_execute_stream_with_object_providers_without_token           | Should_execute_scalar_with_object_providers_without_token
//   3 | CreateAsyncQueryable<T,TS>(stream, data, mapper) [custom, no token]       | Should_execute_stream_with_custom_source_providers_without_token    | Should_execute_scalar_with_custom_source_providers_without_token
//   4 | CreateAsyncQueryable<T>(stream+token, data+token) [DynamicObject]         | Should_execute_stream_with_dynamic_object_providers_with_token      | Should_execute_scalar_with_dynamic_object_providers_with_token
//   5 | CreateAsyncQueryable<T>(stream+token, data+token, mapper) [object]        | Should_execute_stream_with_object_providers_with_token              | Should_execute_scalar_with_object_providers_with_token
//   6 | CreateAsyncQueryable<T,TS>(stream+token, data+token, mapper) [custom]     | Should_execute_stream_with_custom_source_providers_with_token       | Should_execute_scalar_with_custom_source_providers_with_token
#nullable enable
namespace Remote.Linq.Async.Queryable.Tests;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using RemoteLinq = Remote.Linq.Expressions;
using RemoteQueryable = Remote.Linq.RemoteQueryable;
using SystemLinq = System.Linq.Expressions;

public class When_using_remote_queryable_factory_overloads
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

    private sealed class RecordingAsyncQueryResultMapper<T> : IAsyncQueryResultMapper<T>
    {
        public int InvocationCount { get; private set; }

        public CancellationToken LastToken { get; private set; }

        public ValueTask<TResult> MapResultAsync<TResult>(T? source, SystemLinq.Expression expression, CancellationToken cancellation = default)
        {
            InvocationCount++;
            LastToken = cancellation;
            return new ValueTask<TResult>((TResult)(object)source!);
        }
    }

    /// <summary>
    /// Overload 1: <see cref="DynamicObject"/> stream/data providers without cancellation-token parameters.
    /// Stream execution with a stream provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_dynamic_object_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var streamInvocations = 0;
        Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject?>> streamProvider = _ =>
        {
            streamInvocations++;
            return new[] { CreateDynamicObject(1), CreateDynamicObject(2) }.ToAsyncEnumerable();
        };

        var queryable = factory.CreateAsyncQueryable<ItemDto>(streamProvider, asyncDataProvider: null);
        var items = new List<ItemDto>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items.Count.ShouldBe(2);
        items[0].Count.ShouldBe(1);
        items[1].Count.ShouldBe(2);
        streamInvocations.ShouldBe(1);
    }

    /// <summary>
    /// Overload 2: <see cref="object"/> stream/data providers without cancellation-token parameters and the default mapper.
    /// Stream execution with a stream provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_object_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item1 = new SourceItem { Count = 3 };
        var item2 = new SourceItem { Count = 4 };
        var streamInvocations = 0;
        Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> streamProvider = _ =>
        {
            streamInvocations++;
            return new object?[] { item1, item2 }.ToAsyncEnumerable();
        };

        var queryable = factory.CreateAsyncQueryable<SourceItem>(streamProvider, asyncDataProvider: null);
        var items = new List<SourceItem>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items[0].ShouldBeSameAs(item1);
        items[1].ShouldBeSameAs(item2);
        streamInvocations.ShouldBe(1);
    }

    /// <summary>
    /// Overload 3: custom <see cref="SourceItem"/> stream/data providers without cancellation-token parameters and an explicit mapper.
    /// Stream execution with a stream provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_custom_source_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item1 = new SourceItem { Count = 5 };
        var item2 = new SourceItem { Count = 6 };
        var streamInvocations = 0;
        Func<RemoteLinq.Expression, IAsyncEnumerable<SourceItem>> streamProvider = _ =>
        {
            streamInvocations++;
            return new[] { item1, item2 }.ToAsyncEnumerable();
        };
        var mapper = new RecordingAsyncQueryResultMapper<SourceItem>();

        var queryable = factory.CreateAsyncQueryable<SourceItem, SourceItem>(streamProvider, asyncDataProvider: null, mapper);
        var items = new List<SourceItem>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items[0].ShouldBeSameAs(item1);
        items[1].ShouldBeSameAs(item2);
        streamInvocations.ShouldBe(1);
        mapper.InvocationCount.ShouldBe(2);

        // the stream-mapping path invokes the result mapper without the execution token
        mapper.LastToken.ShouldBe(CancellationToken.None);
    }

    /// <summary>
    /// Overload 4: <see cref="DynamicObject"/> stream/data providers with cancellation-token parameters.
    /// Stream execution with a stream provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_dynamic_object_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var streamInvocations = 0;
        CancellationToken? streamToken = null;
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject?>> streamProvider = (_, cancellation) =>
        {
            streamInvocations++;
            streamToken = cancellation;
            return new[] { CreateDynamicObject(7), CreateDynamicObject(8) }.ToAsyncEnumerable();
        };

        var queryable = factory.CreateAsyncQueryable<ItemDto>(streamProvider, asyncDataProvider: null);
        var items = new List<ItemDto>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items.Count.ShouldBe(2);
        items[0].Count.ShouldBe(7);
        items[1].Count.ShouldBe(8);
        streamInvocations.ShouldBe(1);
        streamToken.ShouldBe(token);
    }

    /// <summary>
    /// Overload 5: <see cref="object"/> stream/data providers with cancellation-token parameters and an explicit mapper.
    /// Stream execution with a stream provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_object_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item1 = new SourceItem { Count = 9 };
        var item2 = new SourceItem { Count = 10 };
        var streamInvocations = 0;
        CancellationToken? streamToken = null;
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> streamProvider = (_, cancellation) =>
        {
            streamInvocations++;
            streamToken = cancellation;
            return new object?[] { item1, item2 }.ToAsyncEnumerable();
        };
        var mapper = new RecordingAsyncQueryResultMapper<object>();

        var queryable = factory.CreateAsyncQueryable<SourceItem>(streamProvider, asyncDataProvider: null, mapper);
        var items = new List<SourceItem>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items[0].ShouldBeSameAs(item1);
        items[1].ShouldBeSameAs(item2);
        streamInvocations.ShouldBe(1);
        streamToken.ShouldBe(token);
        mapper.InvocationCount.ShouldBe(2);

        // the stream-mapping path invokes the result mapper without the execution token
        mapper.LastToken.ShouldBe(CancellationToken.None);
    }

    /// <summary>
    /// Overload 6: custom <see cref="SourceItem"/> stream/data providers with cancellation-token parameters and an explicit mapper.
    /// Stream execution with a stream provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_stream_with_custom_source_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item1 = new SourceItem { Count = 11 };
        var item2 = new SourceItem { Count = 12 };
        var streamInvocations = 0;
        CancellationToken? streamToken = null;
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<SourceItem>> streamProvider = (_, cancellation) =>
        {
            streamInvocations++;
            streamToken = cancellation;
            return new[] { item1, item2 }.ToAsyncEnumerable();
        };
        var mapper = new RecordingAsyncQueryResultMapper<SourceItem>();

        var queryable = factory.CreateAsyncQueryable<SourceItem, SourceItem>(streamProvider, asyncDataProvider: null, mapper);
        var items = new List<SourceItem>();
        await foreach (var item in queryable.WithCancellation(token))
        {
            items.Add(item);
        }

        items[0].ShouldBeSameAs(item1);
        items[1].ShouldBeSameAs(item2);
        streamInvocations.ShouldBe(1);
        streamToken.ShouldBe(token);
        mapper.InvocationCount.ShouldBe(2);

        // the stream-mapping path invokes the result mapper without the execution token
        mapper.LastToken.ShouldBe(CancellationToken.None);
    }

    /// <summary>
    /// Overload 1: <see cref="DynamicObject"/> stream/data providers without cancellation-token parameters.
    /// Scalar execution with a data provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_dynamic_object_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var dataInvocations = 0;
        Func<RemoteLinq.Expression, ValueTask<DynamicObject?>> dataProvider = _ =>
        {
            dataInvocations++;
            return new ValueTask<DynamicObject?>(CreateDynamicObject(13));
        };

        var queryable = factory.CreateAsyncQueryable<ItemDto>(asyncStreamProvider: null, dataProvider);
        var single = await queryable.SingleAsync(token);

        single.Count.ShouldBe(13);
        dataInvocations.ShouldBe(1);
    }

    /// <summary>
    /// Overload 2: <see cref="object"/> stream/data providers without cancellation-token parameters and the default mapper.
    /// Scalar execution with a data provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_object_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item = new SourceItem { Count = 14 };
        var dataInvocations = 0;
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider = _ =>
        {
            dataInvocations++;
            return new ValueTask<object?>(item);
        };

        var queryable = factory.CreateAsyncQueryable<SourceItem>(asyncStreamProvider: null, dataProvider);
        var single = await queryable.SingleAsync(token);

        single.ShouldBeSameAs(item);
        dataInvocations.ShouldBe(1);
    }

    /// <summary>
    /// Overload 3: custom <see cref="SourceItem"/> stream/data providers without cancellation-token parameters and an explicit mapper.
    /// Scalar execution with a data provider only: the original non-token delegate is adapted and invoked.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_custom_source_providers_without_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item = new SourceItem { Count = 15 };
        var dataInvocations = 0;
        Func<RemoteLinq.Expression, ValueTask<SourceItem?>> dataProvider = _ =>
        {
            dataInvocations++;
            return new ValueTask<SourceItem?>(item);
        };
        var mapper = new RecordingAsyncQueryResultMapper<SourceItem>();

        var queryable = factory.CreateAsyncQueryable<SourceItem, SourceItem>(asyncStreamProvider: null, dataProvider, mapper);
        var single = await queryable.SingleAsync(token);

        single.ShouldBeSameAs(item);
        dataInvocations.ShouldBe(1);
        mapper.InvocationCount.ShouldBe(1);
        mapper.LastToken.ShouldBe(token);
    }

    /// <summary>
    /// Overload 4: <see cref="DynamicObject"/> stream/data providers with cancellation-token parameters.
    /// Scalar execution with a data provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_dynamic_object_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var dataInvocations = 0;
        CancellationToken? dataToken = null;
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject?>> dataProvider = (_, cancellation) =>
        {
            dataInvocations++;
            dataToken = cancellation;
            return new ValueTask<DynamicObject?>(CreateDynamicObject(16));
        };

        var queryable = factory.CreateAsyncQueryable<ItemDto>(asyncStreamProvider: null, dataProvider);
        var single = await queryable.SingleAsync(token);

        single.Count.ShouldBe(16);
        dataInvocations.ShouldBe(1);
        dataToken.ShouldBe(token);
    }

    /// <summary>
    /// Overload 5: <see cref="object"/> stream/data providers with cancellation-token parameters and an explicit mapper.
    /// Scalar execution with a data provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_object_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item = new SourceItem { Count = 17 };
        var dataInvocations = 0;
        CancellationToken? dataToken = null;
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider = (_, cancellation) =>
        {
            dataInvocations++;
            dataToken = cancellation;
            return new ValueTask<object?>(item);
        };
        var mapper = new RecordingAsyncQueryResultMapper<object>();

        var queryable = factory.CreateAsyncQueryable<SourceItem>(asyncStreamProvider: null, dataProvider, mapper);
        var single = await queryable.SingleAsync(token);

        single.ShouldBeSameAs(item);
        dataInvocations.ShouldBe(1);
        dataToken.ShouldBe(token);
        mapper.InvocationCount.ShouldBe(1);
        mapper.LastToken.ShouldBe(token);
    }

    /// <summary>
    /// Overload 6: custom <see cref="SourceItem"/> stream/data providers with cancellation-token parameters and an explicit mapper.
    /// Scalar execution with a data provider only: the execution token is forwarded to the token-aware delegate.
    /// </summary>
    [Fact]
    public async Task Should_execute_scalar_with_custom_source_providers_with_token()
    {
        var factory = RemoteQueryable.Factory;
        var token = TestContext.Current.CancellationToken;

        var item = new SourceItem { Count = 18 };
        var dataInvocations = 0;
        CancellationToken? dataToken = null;
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<SourceItem?>> dataProvider = (_, cancellation) =>
        {
            dataInvocations++;
            dataToken = cancellation;
            return new ValueTask<SourceItem?>(item);
        };
        var mapper = new RecordingAsyncQueryResultMapper<SourceItem>();

        var queryable = factory.CreateAsyncQueryable<SourceItem, SourceItem>(asyncStreamProvider: null, dataProvider, mapper);
        var single = await queryable.SingleAsync(token);

        single.ShouldBeSameAs(item);
        dataInvocations.ShouldBe(1);
        dataToken.ShouldBe(token);
        mapper.InvocationCount.ShouldBe(1);
        mapper.LastToken.ShouldBe(token);
    }

    private static DynamicObject CreateDynamicObject(int count)
        => new(new[] { new Property("Count", count) });
}
