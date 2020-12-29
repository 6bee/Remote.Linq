// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.AsyncRemoteStream
{
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

        private const string MustImplemenrtAsyncRemoteStreamProvider =
            "The provider for the source IQueryable doesn't implement IAsyncRemoteStreamProvider. " +
            "Only providers implementing Remote.Linq.IAsyncRemoteStreamProvider can be used for Remote Linq's AsAsyncEnumerable operation.";

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
        public void Should_throw_upon_calling_count_on_remote_stream_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.Count());
            ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
        }

        [Fact]
        public void Should_throw_upon_calling_single_on_remote_stream_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.Single(x => x.Id == 1));
            ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
        }

        [Fact]
        public void Should_throw_upon_calling_tolist_on_remote_stream_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => AsyncRemoteStreamQueryable.ToList());
            ex.Message.ShouldBe(MustBeExecutedAsAsyncEnumerable);
        }

        [Fact]
        public void Should_throw_upon_calling_AsAsyncEnumerable_with_nonvalid_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => NonRemoteQueryable.AsAsyncEnumerable());
            ex.Message.ShouldBe(MustImplemenrtAsyncRemoteStreamProvider);
        }
    }
}