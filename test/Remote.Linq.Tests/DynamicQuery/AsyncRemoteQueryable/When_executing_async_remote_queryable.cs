// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.AsyncRemoteQueryable
{
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using static Remote.Linq.Tests.DynamicQuery.EntityQueryables;

    public class When_executing_async_remote_queryable
    {
        private const string MustImplemenrtAsyncRemoteQueryProvider =
            "The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. " +
            "Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.";

        [Fact]
        public void Should_query_full_set()
        {
            var count = 0;
            foreach (var item in AsyncRemoteQueryable)
            {
                count++;
            }

            count.ShouldBe(10);
        }

        [Fact]
        public void Should_query_filtered_set()
        {
            var count = 0;
            foreach (var item in FilteredAsyncRemoteQueryable)
            {
                count++;
            }

            count.ShouldBe(3);
        }

        [Fact]
        public void Should_query_single_item()
        {
            var query =
                from item in AsyncRemoteQueryable
                where item.Id == 5
                select item;

            var sum = 0;
            foreach (var item in query)
            {
                sum += item.Id;
            }

            sum.ShouldBe(5);
        }

        [Fact]
        public async Task Should_query_countasync_on_remote_queryable()
        {
            var count = await FilteredAsyncRemoteQueryable.CountAsync();
            count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_query_singleasync_on_remote_queryable()
        {
            var item = await AsyncRemoteQueryable.SingleAsync(x => x.Id == 1);
            item.Id.ShouldBe(1);
        }

        [Fact]
        public async Task Should_query_tolistasync_on_remote_queryable()
        {
            var result = await FilteredAsyncRemoteQueryable.ToListAsync();
            result.Count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_query_toarrayasync_on_remote_queryable()
        {
            var result = await FilteredAsyncRemoteQueryable.ToArrayAsync();
            result.Length.ShouldBe(3);
        }

        [Fact]
        public async Task Should_throw_upon_calling_countasync_on_non_remote_queryable()
        {
            var ex = await Should.ThrowAsync<NotSupportedException>(() => NonRemoteQueryable.CountAsync());
            ex.Message.ShouldBe(MustImplemenrtAsyncRemoteQueryProvider);
        }

        [Fact]
        public void Should_throw_upon_calling_singleasync_on_non_remote_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => NonRemoteQueryable.SingleAsync(x => x.Id == 1));
            ex.Message.ShouldBe(MustImplemenrtAsyncRemoteQueryProvider);
        }

        [Fact]
        public void Should_throw_upon_calling_tolistasync_on_non_remote_queryable()
        {
            var ex = Should.Throw<NotSupportedException>(() => NonRemoteQueryable.ToListAsync());
            ex.Message.ShouldBe(MustImplemenrtAsyncRemoteQueryProvider);
        }

        [Fact]
        public async Task Should_throw_upon_calling_toarrayasync_on_non_remote_queryable()
        {
            var ex = await Should.ThrowAsync<NotSupportedException>(() => NonRemoteQueryable.ToArrayAsync());
            ex.Message.ShouldBe(MustImplemenrtAsyncRemoteQueryProvider);
        }
    }
}