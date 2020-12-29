// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.Tests
{
    using Remote.Linq.Async.Queryable.TestSupport;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public sealed class When_querying_async
    {
        private static IAsyncQueryable<Entity> AsyncQueryable =
            Enumerable.Range(0, 10)
            .Select(x => new Entity(x))
            .AsAsyncQueryable();

        [Fact]
        public async Task Should_query_full_set_via_async_stream()
        {
            var count = 0;
            await foreach (var item in AsyncQueryable)
            {
                count++;
            }

            count.ShouldBe(10);
        }

        [Fact]
        public async Task Should_query_full_set_via_foreach_async()
        {
            var count = 0;
            await AsyncQueryable.ForEachAsync(_ => count++);

            count.ShouldBe(10);
        }

        [Fact]
        public async Task Should_get_count_via_async_stream()
        {
            var count = await AsyncQueryable.AsAsyncEnumerable().CountAsync();

            count.ShouldBe(10);
        }

        [Fact]
        public async Task Should_query_filtered_set_as_async_stream()
        {
            var query =
                from x in AsyncQueryable
                where x.Id >= 5 && x.Id <= 7
                select x;

            var count = 0;
            await foreach (var item in query)
            {
                count++;
            }

            count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_query_async_single_item()
        {
            var single = await AsyncQueryable
                .AsAsyncEnumerable()
                .SingleAsync(x => x.Id == 5)
                .ConfigureAwait(false);
            single.Id.ShouldBe(5);
        }

        [Fact]
        public async Task Should_query_async_count()
        {
            var count = await AsyncQueryable.AsAsyncEnumerable().CountAsync();
            count.ShouldBe(10);
        }

        [Fact]
        public async Task Should_throw_upon_calling_single_linq_extension()
        {
            await AssertThrowNotSupportedExceptionAsync(async () => await AsyncQueryable.SingleAsync(x => x.Id == 1));
        }

        [Fact]
        public async Task Should_throw_upon_calling_tolist_linq_extension()
        {
            await AssertThrowNotSupportedExceptionAsync(async () => await AsyncQueryable.ToListAsync());
        }

        [Fact]
        public async Task Should_throw_upon_calling_toarrayasync_linq_extension()
        {
            await AssertThrowNotSupportedExceptionAsync(async () => await AsyncQueryable.ToArrayAsync());
        }

        private static async Task AssertThrowNotSupportedExceptionAsync(Func<Task> task)
        {
            var ex = await Should.ThrowAsync<NotSupportedException>(task);
            ex.Message.ShouldBe("Async remote stream may only be executed as IAsyncEnumerable<>. " +
                "Consider calling AsAsyncEnumerable() extension method before enumerating.");
        }

        [Fact]
        public void Should_throw_upon_calling_AsAsyncEnumerable_with_non_remote_queryable()
        {
            var source = Enumerable.Empty<Entity>().AsQueryable();
            var ex = Should.Throw<NotSupportedException>(() => source.AsAsyncEnumerable());
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteStreamProvider. " +
                "Only providers implementing Remote.Linq.IAsyncRemoteStreamProvider can be used for Remote Linq's AsAsyncEnumerable operation.");
        }
    }
}
