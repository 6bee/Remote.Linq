// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if COREFX

namespace Remote.Linq.Tests.DynamicQuery.AsyncRemoteStream
{
    using Remote.Linq;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.TestSupport;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class When_executing_async_remote_stream
    {
        private class Entity
        {
            public Entity(int id)
            {
                Id = id;
            }

            public int Id { get; set; }
        }

        private readonly IQueryable<Entity> _queryable;

        public When_executing_async_remote_stream()
        {
            _queryable = Enumerable.Range(0, 10)
                .Select(x => new Entity(x))
                .AsAsyncRemoteStreamQueryable();
        }

        [Fact]
        public async Task Should_query_full_set()
        {
            var count = 0;
            await foreach (var item in _queryable.AsAsyncEnumerable())
            {
                count++;
            }

            count.ShouldBe(10);
        }

        [Fact]
        public async Task Should_query_filtered_set()
        {
            var query =
                from x in _queryable
                where x.Id >= 5 && x.Id <= 7
                select x;

            var count = 0;
            await foreach (var item in query.AsAsyncEnumerable())
            {
                count++;
            }

            count.ShouldBe(3);
        }

        [Fact]
        public async Task Should_query_single_item()
        {
            var query = _queryable.Where(x => x.Id == 5);

            var sum = 0;
            await foreach (var item in query.AsAsyncEnumerable())
            {
                sum += item.Id;
            }

            sum.ShouldBe(5);
        }

        [Fact]
        public void Should_throw_upon_calling_count_linq_extension()
        {
            var ex = Should.Throw<QueryOperationNotSupportedException>(() => _queryable.Count());
            ex.Message.ShouldBe("Async remote stream must be executed as IAsyncEnumerable<T>. The AsAsyncEnumerable() extension method may be used.");
        }

        [Fact]
        public void Should_throw_upon_calling_single_linq_extension()
        {
            var ex = Should.Throw<QueryOperationNotSupportedException>(() => _queryable.Single(x => x.Id == 1));
            ex.Message.ShouldBe("Async remote stream must be executed as IAsyncEnumerable<T>. The AsAsyncEnumerable() extension method may be used.");
        }

        [Fact]
        public void Should_throw_upon_calling_tolist_linq_extension()
        {
            var ex = Should.Throw<QueryOperationNotSupportedException>(() => _queryable.ToList());
            ex.Message.ShouldBe("Async remote stream must be executed as IAsyncEnumerable<T>. The AsAsyncEnumerable() extension method may be used.");
        }

        [Fact]
        public async Task Should_throw_upon_calling_toarrayasync_linq_extension()
        {
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.ToArrayAsync()).ConfigureAwait(false);
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public void Should_throw_upon_calling_AsAsyncEnumerable_with_nonvalid_queryable()
        {
            var source = Enumerable.Empty<Entity>().AsQueryable();
            var ex = Should.Throw<InvalidOperationException>(() => source.AsAsyncEnumerable());
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteStreamProvider. Only providers implementing Remote.Linq.IAsyncRemoteStreamProvider can be used for Remote Linq's AsAsyncEnumerable operation.");
        }
    }
}

#endif // COREFX