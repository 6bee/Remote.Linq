﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests
{
    using Remote.Linq.Async;
    using Remote.Linq.EntityFrameworkCore.Tests.Model;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using DbFunctionsExtensions = Microsoft.EntityFrameworkCore.DbFunctionsExtensions;

    public sealed class When_querying : IDisposable
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

            _queryable = RemoteQueryable.Factory.CreateAsyncQueryable<LookupItem>(x => x.ExecuteWithEntityFrameworkCoreAsync(_context));
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public async Task Should_not_evaluate_ef_functions_prematurely()
        {
            var result = await _queryable
                .Where(p => DbFunctionsExtensions.Like(null, p.Value, "%e")) // EF.Functions.Like(<property>, <pattern>)
                .ToListAsync()
                .ConfigureAwait(false);
            result.Count.ShouldBe(2);
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
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.SingleOrDefaultAsync(x => x.Value.ToUpper().Contains("O")).AsTask()).ConfigureAwait(false);
            ex.Message.ShouldBe("Sequence contains more than one matching element");
        }

        [Fact]
        public void Single_with_predicate_should_faile_if_no_match()
        {
            var ex = Should.Throw<InvalidOperationException>(() => _queryable.Single(x => x.Value.ToUpper().Contains("no match")));
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public async Task SingleAsync_with_predicate_should_faile_if_no_match()
        {
            var ex = await Should.ThrowAsync<InvalidOperationException>(() => _queryable.SingleAsync(x => x.Value.ToUpper().Contains("no match")).AsTask());
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void SingleOrDefault_with_predicate_should_return_null_if_no_match()
        {
            var result = _queryable.SingleOrDefault(x => x.Value.ToUpper().Contains("no match"));
            result.ShouldBeNull();
        }

        [Fact]
        public async Task SingleOrDefaultAsync_with_predicate_should_return_null_if_no_match()
        {
            var result = await _queryable.SingleOrDefaultAsync(x => x.Value.ToUpper().Contains("no match")).ConfigureAwait(false);
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_throw_when_calling_ElementAt()
        {
            Should.Throw<InvalidOperationException>(() => _queryable.ElementAt(0));
        }

        [Fact]
        public void Should_throw_when_calling_ElementAtOrDefault()
        {
            Should.Throw<InvalidOperationException>(() => _queryable.ElementAtOrDefault(0));
        }
    }
}
