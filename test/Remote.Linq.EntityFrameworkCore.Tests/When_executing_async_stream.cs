// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if COREFX

namespace Remote.Linq.EntityFrameworkCore.Tests
{
    using Remote.Linq;
    using Remote.Linq.EntityFrameworkCore.Tests.Model;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class When_executing_async_stream : IDisposable
    {
        private readonly TestContext _context;
        private readonly IQueryable<LookupItem> _queryable;

        [SecuritySafeCritical]
        public When_executing_async_stream()
        {
            _context = new TestContext();
            _context.Lookup.Add(new LookupItem { Key = "1", Value = "One" });
            _context.Lookup.Add(new LookupItem { Key = "2", Value = "Two" });
            _context.Lookup.Add(new LookupItem { Key = "3", Value = "Three" });
            _context.SaveChanges();

            _queryable = RemoteQueryable.Factory.CreateQueryable<LookupItem>(expression => expression.ExecuteAsyncStreamWithEntityFrameworkCore(_context));
        }

        public void Dispose() => _context.Dispose();

        private static async Task<T[]> ToArrayAsync<T>(IAsyncEnumerable<T> asyncResultStream)
        {
            var list = new List<T>();
            await foreach (var item in asyncResultStream)
            {
                list.Add(item);
            }

            return list.ToArray();
        }

        [Fact]
        public async Task Should_query_single_with_predicate()
        {
            var asyncResultStream = _queryable
                .Where(x => x.Value.ToUpper().Contains("W"))
                .AsAsyncEnumerable();
            var result = await ToArrayAsync(asyncResultStream).ConfigureAwait(false);
            result.Single().Key.ShouldBe("2");
        }

        [Fact]
        public async Task Should_yield_results_for_query_with_predicate_with_multiple_results()
        {
            var asyncResultStream = _queryable
                .Where(x => x.Value.ToUpper().Contains("O"))
                .OrderByDescending(x => x.Key)
                .AsAsyncEnumerable();
            var result = await ToArrayAsync(asyncResultStream).ConfigureAwait(false);
            result.Length.ShouldBe(2);
            result[0].Key.ShouldBe("2");
            result[1].Key.ShouldBe("1");
        }

        [Fact]
        public async Task Should_yield_empty_result_for_query_with_predicate_with_no_match()
        {
            var asyncResultStream = _queryable
                .Where(x => x.Value.ToUpper().Contains("no match"))
                .AsAsyncEnumerable();
            var result = await ToArrayAsync(asyncResultStream).ConfigureAwait(false);
            result.ShouldBeEmpty();
        }

        [Fact]
        public void Should_throw_for_scalar_query_execution()
        {
            var ex = Should.Throw<InvalidOperationException>(() => _queryable.SingleOrDefault());
            ex.Message.ShouldBe("Async remote stream must be executed as IAsyncEnumerable<T>. The AsAsyncEnumerable() extension method may be used.");
        }
    }
}

#endif // COREFX
