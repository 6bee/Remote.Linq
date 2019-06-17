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
        private IQueryable<int> _queryable = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1001, 1002, 1004, -1, -2, -5 }.AsQueryable();

        [Fact]
        public async Task Should_throw_with_enumerable_query()
        {
            var exception = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.ToListAsync()).ConfigureAwait(false);
            exception.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public async Task Should_throw_with_non_async_remote_queriable()
        {
            var remoteQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => _queryable));

            var exception = await Should.ThrowAsync<InvalidOperationException>(() => remoteQueryable.ToListAsync()).ConfigureAwait(false);
            exception.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public async Task Should_execute_async_remote_queriable()
        {
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateAsyncQueryable<int>(x => Task.Run(() => x.Execute(t => _queryable)));

            var result = await remoteAsyncQueryable.ToListAsync().ConfigureAwait(false);

            result.ShouldMatch(_queryable.ToList());
        }

        [Fact]
        public async Task Should_throw_with_enumerable_query_scalar()
        {
            var exception = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.AverageAsync()).ConfigureAwait(false);
            exception.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public async Task Should_throw_with_non_async_remote_queriable_scalar()
        {
            var remoteQueryable = RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => _queryable));

            var exception = await Should.ThrowAsync<InvalidOperationException>(() => remoteQueryable.AverageAsync()).ConfigureAwait(false);
            exception.Message.ShouldBe("The provider for the source IQueryable doesn't implement IAsyncRemoteQueryProvider. Only providers implementing Remote.Linq.IAsyncRemoteQueryProvider can be used for Remote Linq asynchronous operations.");
        }

        [Fact]
        public async Task Should_execute_async_remote_queriable_scalar()
        {
            var remoteAsyncQueryable = RemoteQueryable.Factory.CreateAsyncQueryable<int>(x => Task.Run(() => x.Execute(t => _queryable)));

            var result = await remoteAsyncQueryable.AverageAsync().ConfigureAwait(false);

            result.ShouldBe(_queryable.Average());
        }
    }
}
