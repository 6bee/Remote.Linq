// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncQueryableExtensions
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Async;
    using Remote.Linq.Expressions;
    using Remote.Linq.Tests;
    using Shouldly;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public abstract class When_using_async_remote_operations
    {
        public class With_non_async_system_queryable : When_using_async_remote_operations
        {
            protected override IQueryable<int> Queryable => Source.AsQueryable();
        }

        public class With_non_async_remote_queryable : When_using_async_remote_operations
        {
            protected override IQueryable<int> Queryable => RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => Source.AsQueryable()));
        }

        public class With_async_remote_queryable : When_using_async_remote_operations
        {
            protected override IQueryable<int> Queryable => RemoteQueryable.Factory.CreateAsyncQueryable<int>(x => new ValueTask<DynamicObject>(x.Execute(t => Source.AsQueryable())));
        }

        protected abstract IQueryable<int> Queryable { get; }

        protected IEnumerable<int> Source => new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1001, 1002, 1004, -1, -2, -5 };

        [Fact]
        public async Task Should_allow_ToListAsync()
        {
            var result = await Queryable.ToListAsync().ConfigureAwait(false);

            result.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_allow_ToArrayAsync()
        {
            var result = await Queryable.ToArrayAsync().ConfigureAwait(false);

            result.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_allow_AverageAsync()
        {
            var result = await Queryable.AverageAsync().ConfigureAwait(false);

            result.ShouldBe(Source.Average());
        }

        [Fact]
        public async Task Should_allow_SumAsync()
        {
            var result = await Queryable.SumAsync().ConfigureAwait(false);

            result.ShouldBe(Source.Sum());
        }

        [Fact]
        public async Task Should_allow_AsAsyncEnumerable()
        {
            var asyncEenumerable = Queryable.Where(x => false).AsAsyncEnumerable();

            var moveNext = await asyncEenumerable.GetAsyncEnumerator().MoveNextAsync().ConfigureAwait(false);

            moveNext.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_allow_CountAsync()
        {
            var resut = await Queryable.CountAsync().ConfigureAwait(false);

            resut.ShouldBe(Source.Count());
        }

        [Fact]
        public async Task Should_allow_SingleAsync_with_predicate()
        {
            var result = await Queryable.SingleAsync(x => x == 1).ConfigureAwait(false);

            result.ShouldBe(1);
        }
    }
}
