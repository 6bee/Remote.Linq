// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.AsyncQueryableExtensions
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Async;
    using Remote.Linq.ExpressionExecution;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public abstract class When_using_async_linq_operations
    {
        public class With_non_async_system_queryable : When_using_async_linq_operations
        {
            protected override IQueryable<int> Queryable => Source.AsQueryable();
        }

        public class With_non_async_remote_queryable : When_using_async_linq_operations
        {
            protected override IQueryable<int> Queryable => RemoteQueryable.Factory.CreateQueryable<int>(x => x.Execute(t => Source.AsQueryable()));
        }

        public class With_async_remote_queryable : When_using_async_linq_operations
        {
            protected override IQueryable<int> Queryable => RemoteQueryable.Factory.CreateAsyncQueryable<int>(x => new ValueTask<DynamicObject>(x.Execute(t => Source.AsQueryable())));
        }

        // NOTE: in practice this type requires to exists on remote side too.
        private class Int32TestComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y) => x == y;

            public int GetHashCode(int obj) => obj is int n ? n.GetHashCode() : 0;
        }

        protected IEnumerable<int> Source => new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1001, 1002, 1004, -1, -2, -5 };

        protected abstract IQueryable<int> Queryable { get; }

        [Fact]
        public async Task Should_perform_ToListAsync()
        {
            var result = await Queryable.ToListAsync();

            result.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_ToArrayAsync()
        {
            var result = await Queryable.ToArrayAsync();

            result.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_ToDictionaryAsync_with_key_selector()
        {
            var result = await Queryable.ToDictionaryAsync(x => x);

            result.Keys.ShouldBeSequenceEqual(Source);
            result.Values.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_ToDictionaryAsync_with_key_and_element_selector()
        {
            var result = await Queryable.ToDictionaryAsync(x => x, x => x);

            result.Keys.ShouldBeSequenceEqual(Source);
            result.Values.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_ToDictionaryAsync_with_key_selector_and_equality_comparer()
        {
            var result = await Queryable.ToDictionaryAsync(x => x, new Int32TestComparer());

            result.Keys.ShouldBeSequenceEqual(Source);
            result.Values.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_ToDictionaryAsync_with_key_and_element_selector_and_equality_comparer()
        {
            var result = await Queryable.ToDictionaryAsync(x => x, x => x, new Int32TestComparer());

            result.Keys.ShouldBeSequenceEqual(Source);
            result.Values.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_AsAsyncEnumerable()
        {
            var asyncEenumerable = Queryable.AsAsyncEnumerable();

            var asyncEnumerator = asyncEenumerable.GetAsyncEnumerator();

            var list = new List<int>();
            while (await asyncEnumerator.MoveNextAsync())
            {
                list.Add(asyncEnumerator.Current);
            }

            list.ShouldBeSequenceEqual(Source);
        }

        [Fact]
        public async Task Should_perform_AggregateAsync()
        {
            var result = await Queryable.AggregateAsync((x, y) => x + y);

            result.ShouldBe(Source.Sum());
        }

        [Fact]
        public async Task Should_perform_AggregateAsync_with_seed()
        {
            var result = await Queryable.AggregateAsync(9999, (x, y) => x + y);

            result.ShouldBe(Source.Sum() + 9999);
        }

        [Fact]
        public async Task Should_perform_AggregateAsync_with_seed_and_selector()
        {
            var result = await Queryable.AggregateAsync(9999, (x, y) => x + y, x => x);

            result.ShouldBe(Source.Sum() + 9999);
        }

        [Fact]
        public async Task Should_perform_FirstAsync()
        {
            var result = await Queryable.FirstAsync();

            result.ShouldBe(Source.First());
        }

        [Fact]
        public async Task Should_perform_FirstAsync_with_predicate()
        {
            var result = await Queryable.FirstAsync(x => x < 0);

            result.ShouldBe(Source.First(x => x < 0));
        }

        [Fact]
        public async Task Should_perform_FirstOrDefaultAsync()
        {
            var result = await Queryable.FirstOrDefaultAsync();

            result.ShouldBe(Source.FirstOrDefault());
        }

        [Fact]
        public async Task Should_perform_FirstOrDefaultAsync_with_predicate()
        {
            var result = await Queryable.FirstOrDefaultAsync(x => x < 0);

            result.ShouldBe(Source.FirstOrDefault(x => x < 0));
        }

        [Fact]
        public async Task Should_perform_SingleAsynce()
        {
            var ex = await Should.ThrowAsync<InvalidOperationException>(async () => await Queryable.SingleAsync());
            ex.Message.ShouldBe("Sequence contains more than one element");
        }

        [Fact]
        public async Task Should_perform_SingleAsync_with_predicate()
        {
            var result = await Queryable.SingleAsync(x => x == 1001);

            result.ShouldBe(1001);
        }

        [Fact]
        public async Task Should_perform_SingleOrDefaultAsync()
        {
            var ex = await Should.ThrowAsync<InvalidOperationException>(async () => await Queryable.SingleOrDefaultAsync());
            ex.Message.ShouldBe("Sequence contains more than one element");
        }

        [Fact]
        public async Task Should_perform_SingleOrDefaultAsync_with_predicate()
        {
            var result = await Queryable.SingleOrDefaultAsync(x => x == 1001);

            result.ShouldBe(1001);
        }

        [Fact]
        public async Task Should_perform_LastAsync()
        {
            var result = await Queryable.LastAsync();

            result.ShouldBe(Source.Last());
        }

        [Fact]
        public async Task Should_perform_LastAsync_with_predicate()
        {
            var result = await Queryable.LastAsync(x => x < 0);

            result.ShouldBe(Source.Last(x => x < 0));
        }

        [Fact]
        public async Task Should_perform_LastOrDefaultAsync()
        {
            var result = await Queryable.LastOrDefaultAsync();

            result.ShouldBe(Source.LastOrDefault());
        }

        [Fact]
        public async Task Should_perform_LastOrDefaultAsync_with_predicate()
        {
            var result = await Queryable.LastOrDefaultAsync(x => x < 0);

            result.ShouldBe(Source.LastOrDefault(x => x < 0));
        }

        [Fact]
        public async Task Should_perform_ElementAtAsync()
        {
            var result = await Queryable.ElementAtAsync(10);

            result.ShouldBe(Source.ElementAt(10));
        }

        [Fact]
        public async Task Should_perform_ElementAtOrDefaultAsync()
        {
            var result = await Queryable.ElementAtOrDefaultAsync(10);

            result.ShouldBe(Source.ElementAtOrDefault(10));
        }

        [Fact]
        public async Task Should_perform_ContainsAsync()
        {
            var result = await Queryable.ContainsAsync(1001);

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_perform_ContainsAsync_with_equality_comparer()
        {
            var result = await Queryable.ContainsAsync(1001, new Int32TestComparer());

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_perform_SequenceEqualAsync()
        {
            var result = await Queryable.SequenceEqualAsync(new[] { 1, 1001 });

            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_perform_SequenceEqualAsync_with_equality_comparer()
        {
            var result = await Queryable.SequenceEqualAsync(new[] { 1, 1001 }, new Int32TestComparer());

            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_perform_AnyAsync()
        {
            var result = await Queryable.AnyAsync();

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_perform_AnyAsync_with_predicate()
        {
            var result = await Queryable.AnyAsync(x => x > 9999);

            result.ShouldBeFalse();
        }

        [Fact]
        public async Task Should_perform_AllAsync()
        {
            var result = await Queryable.AllAsync(x => x < 9999);

            result.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_perform_CountAsync()
        {
            var resut = await Queryable.CountAsync();

            resut.ShouldBe(Source.Count());
        }

        [Fact]
        public async Task Should_perform_CountAsync_with_predicate()
        {
            var resut = await Queryable.CountAsync(x => x > 1000);

            resut.ShouldBe(Source.Count(x => x > 1000));
        }

        [Fact]
        public async Task Should_perform_LongCountAsync()
        {
            var resut = await Queryable.LongCountAsync();

            resut.ShouldBe(Source.LongCount());
        }

        [Fact]
        public async Task Should_perform_LongCountAsync_with_predicate()
        {
            var resut = await Queryable.LongCountAsync(x => x > 1000);

            resut.ShouldBe(Source.LongCount(x => x > 1000));
        }

        [Fact]
        public async Task Should_perform_MinAsync()
        {
            var resut = await Queryable.MinAsync();

            resut.ShouldBe(Source.Min());
        }

        [Fact]
        public async Task Should_perform_MinAsync_with_predicate()
        {
            var resut = await Queryable.MinAsync(x => x > 1000);

            resut.ShouldBe(Source.Min(x => x > 1001));
        }

        [Fact]
        public async Task Should_perform_MaxAsync()
        {
            var resut = await Queryable.MaxAsync();

            resut.ShouldBe(Source.Max());
        }

        [Fact]
        public async Task Should_perform_MaxAsync_with_predicate()
        {
            var resut = await Queryable.MaxAsync(x => x > 1000);

            resut.ShouldBe(Source.Max(x => x > 1001));
        }

        [Fact]
        public async Task Should_perform_SumAsync()
        {
            var resut = await Queryable.SumAsync();

            resut.ShouldBe(Source.Sum());
        }

        [Fact]
        public async Task Should_perform_SumAsync_with_selector()
        {
            var resut = await Queryable.SumAsync(x => x % 2);

            resut.ShouldBe(Source.Count(x => x % 2 == 1) - Source.Count(x => x % 2 == -1));
        }

        [Fact]
        public async Task Should_perform_AverageAsync()
        {
            var resut = await Queryable.AverageAsync();

            resut.ShouldBe(Source.Average());
        }

        [Fact]
        public async Task Should_perform_AverageAsync_with_selector()
        {
            var resut = await Queryable.AverageAsync(x => x);

            resut.ShouldBe(Source.Average(x => x));
        }

        [Fact]
        public async Task Should_perform_ExecuteAsync()
        {
            var resut = await Queryable.ExecuteAsync();

            resut.ShouldBeSequenceEqual(Source);
        }
    }
}
