// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncEnumerableExtensions
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
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
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => Task.FromResult(x.Execute(t => _queryable)));

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
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => Task.FromResult(x.Execute(t => _queryable)));

            var result = await remoteAsyncQueryable.AverageAsync().ConfigureAwait(false);

            result.ShouldBe(_queryable.Average());
        }

        private static async Task AssertThrowNotSupportedExceptionAsync(Func<Task> task)
        {
            var ex = await Should.ThrowAsync<NotSupportedException>(task);
            ex.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. " +
                "Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }
    }
}
