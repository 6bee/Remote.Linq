// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.AsyncRemoteStream;

using Remote.Linq.Async;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Remote.Linq.Tests.DynamicQuery.EntityQueryables;

public class When_executing_async_remote_stream
{
    private const string MustBeExecutedAsAsyncEnumerable =
        "Async remote stream must be executed as IAsyncEnumerable<T>. The AsAsyncEnumerable() extension method may be used.";

    [Fact]
    public async Task Should_query_full_set()
    {
        var count = 0;
        await foreach (var item in AsyncRemoteStreamQueryable.AsAsyncEnumerable())
        {
            count++;
        }

        count.ShouldBe(10);
    }

    [Fact]
    public async Task Should_query_filtered_set()
    {
        var count = 0;
        await foreach (var item in FilteredAsyncRemoteStreamQueryable.AsAsyncEnumerable())
        {
            count++;
        }

        count.ShouldBe(3);
    }

    [Fact]
    public async Task Should_query_single_item()
    {
        var query = AsyncRemoteStreamQueryable.Where(x => x.Id == 5);

        var sum = 0;
        await foreach (var item in query.AsAsyncEnumerable())
        {
            sum += item.Id;
        }

        sum.ShouldBe(5);
    }

    [Fact]
    public void Should_throw_upon_calling_Count_on_remote_stream_queryable()
    {
        var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.Count());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public async Task Should_throw_upon_calling_CountAsync_on_remote_stream_queryable()
    {
        var ex = await Should.ThrowAsync<NotSupportedException>(async () => await AsyncRemoteStreamQueryable.CountAsync());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public void Should_throw_upon_calling_Single_on_remote_stream_queryable()
    {
        var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.Single(x => x.Id == 1));
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public async Task Should_throw_upon_calling_SingleAsync_on_remote_stream_queryable()
    {
        var ex = await Should.ThrowAsync<NotSupportedException>(async () => await AsyncRemoteStreamQueryable.SingleAsync(x => x.Id == 1));
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public void Should_throw_upon_calling_ToArray_on_remote_stream_queryable()
    {
        var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.ToArray());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public async Task Should_throw_upon_calling_ToArrayAsync_on_remote_stream_queryable()
    {
        var ex = await Should.ThrowAsync<NotSupportedException>(async () => await AsyncRemoteStreamQueryable.ToArrayAsync());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public void Should_throw_upon_calling_tolist_on_remote_stream_queryable()
    {
        var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.ToList());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }

    [Fact]
    public async Task Should_throw_upon_calling_ToListAsync_on_remote_stream_queryable()
    {
        var ex = await Should.ThrowAsync<NotSupportedException>(async () => await AsyncRemoteStreamQueryable.ToListAsync());
        ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
    }
}