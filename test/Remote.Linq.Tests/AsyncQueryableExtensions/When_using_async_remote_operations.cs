// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncEnumerableExtensions
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class When_using_async_remote_operations
    {
        private readonly IQueryable<int> _queryable = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1001, 1002, 1004, -1, -2, -5 }.AsQueryable();

        [Fact]
        public async Task Should_throw_with_enumerable_query()
        {
            await AssertThrowNotSupportedExceptionAsync(() => _queryable.ToListAsync());
        }

        [Fact]
        public async Task Should_throw_with_non_async_remote_queriable()
        {
            var remoteQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => _queryable));

            await AssertThrowNotSupportedExceptionAsync(() => remoteQueryable.ToListAsync());
        }

        [Fact]
        public async Task Should_execute_async_remote_queriable()
        {
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => new ValueTask<IEnumerable<DynamicObject>>(x.Execute(t => _queryable)));

            var result = await remoteAsyncQueryable.ToListAsync().ConfigureAwait(false);

            result.ShouldMatch(_queryable.ToList());
        }

        [Fact]
        public async Task Should_throw_with_enumerable_query_scalar()
        {
            await AssertThrowNotSupportedExceptionAsync(() => _queryable.AverageAsync());
        }

        [Fact]
        public async Task Should_throw_with_non_async_remote_queriable_scalar()
        {
            var remoteQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => _queryable));

            await AssertThrowNotSupportedExceptionAsync(() => remoteQueryable.AverageAsync());
        }

        [Fact]
        public async Task Should_execute_async_remote_queriable_scalar()
        {
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => new ValueTask<IEnumerable<DynamicObject>>(x.Execute(t => _queryable)));

            var result = await remoteAsyncQueryable.AverageAsync().ConfigureAwait(false);

            result.ShouldBe(_queryable.Average());
        }

        private static async Task AssertThrowNotSupportedExceptionAsync<T>(Func<ValueTask<T>> task)
        {
            var ex = await Should.ThrowAsync<NotSupportedException>(() => task().AsTask());
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. " +
                "Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public void Should_throw_upon_calling_AsAsyncEnumerable_with_non_remote_queryable()
        {
            var source = Enumerable.Empty<int>().AsQueryable();
            var ex = Should.Throw<NotSupportedException>(() => source.AsAsyncEnumerable());
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteStreamProvider. " +
                "Only providers implementing Remote.Linq.IAsyncRemoteStreamProvider can be used for Remote Linq's AsAsyncEnumerable operation.");
        }
    }
}
