// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests
{
    using Remote.Linq;
    using Remote.Linq.EntityFrameworkCore.Tests.Model;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class When_querying : IDisposable
    {
        private readonly TestContext _context;
        private readonly IQueryable<LookupItem> _queryable;

        public When_querying()
        {
            _context = new TestContext();
            _context.Lookup.Add(new LookupItem { Key = "1", Value = "One" });
            _context.Lookup.Add(new LookupItem { Key = "2", Value = "Two" });
            _context.Lookup.Add(new LookupItem { Key = "3", Value = "Three" });
            _context.SaveChanges();

            _queryable = RemoteQueryable.Factory.CreateQueryable<LookupItem>(x => x.ExecuteWithEntityFrameworkCoreAsync(_context));
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Should_query_single_with_predicate()
        {
            var result = _queryable.Single(x => x.Value.ToUpper().Contains("W"));
            result.Key.ShouldBe("2");
        }

        [Fact]
        public async Task Should_query_single_with_predicate_async()
        {
            var result = await _queryable.SingleAsync(x => x.Value.ToUpper().Contains("W")).ConfigureAwait(false);
            result.Key.ShouldBe("2");
        }

        [Fact]
        public void Should_throw_when_calling_single_or_default_with_predicate_on_query_with_multiple_results()
        {
            var ex = Should.Throw<InvalidOperationException>(() => _queryable.SingleOrDefault(x => x.Value.ToUpper().Contains("O")));
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }

        [Fact]
        public async Task Should_throw_when_calling_single_or_default_with_predicate_on_query_with_multiple_results_async()
        {
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.SingleOrDefaultAsync(x => x.Value.ToUpper().Contains("O"))).ConfigureAwait(false);
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }
    }
}
