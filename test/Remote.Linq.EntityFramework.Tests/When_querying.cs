// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.Tests
{
    using Remote.Linq;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.EntityFramework.Tests.TestInfrastructure;
    using Remote.Linq.EntityFramework.Tests.TestModel;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public partial class When_querying : IDisposable
    {
        private readonly TestContext _context;
        private readonly IQueryable<LookupItem> _queryable;

        public When_querying()
        {
            var data = new List<LookupItem>
                {
                    new LookupItem { Key = "1", Value = "One" },
                    new LookupItem { Key = "2", Value = "Two" },
                    new LookupItem { Key = "3", Value = "Three" },
                };
            _context = new ContextMock<TestContext>()
                .WithSet(x => x.Items, data)
                .Object;
            _queryable = RemoteQueryable.Factory.CreateQueryable<LookupItem>(x => x.ExecuteWithEntityFrameworkAsync(_context));
        }

        public void Dispose() => _context.Dispose();

        [Fact]
        public void Should_query_single_with_predicate()
        {
            var result = _queryable.Single(x => x.Value.ToUpper().Contains("W"));
            result.Key.ShouldBe("2");
        }

        [Fact]
        public void Should_query_single_or_default_with_predicate()
        {
            var result = _queryable.SingleOrDefault(x => x.Value.ToUpper().Contains("asdfasdfasdfW"));
            result.ShouldBeNull();
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

        [Fact]
        public void Single_with_predicate_should_faile_if_no_match()
        {
            var ex = Should.Throw<InvalidOperationException>(() => _queryable.Single(x => x.Value.ToUpper().Contains("no match")));
            ex.Message.ShouldBe("Sequence contains no matching element");
        }

        [Fact]
        public void SingleAsync_with_predicate_should_faile_if_no_match()
        {
            var ex = Should.Throw<InvalidOperationException>(() => _queryable.SingleAsync(x => x.Value.ToUpper().Contains("no match")));
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
        private async Task Should_handle_non_materialized_ienumerable_closure()
        {
            var data = new List<LookupItem>
            {
                new LookupItem { Key = "1", Value = "One" },
                new LookupItem { Key = "2", Value = "Two" },
                new LookupItem { Key = "3", Value = "Three" },
            };
            var filteredPeoplesNames = data.Where(x => x.Value.StartsWith("O")).Select(x => x.Value);
            var result = await _queryable.FirstOrDefaultAsync(x => filteredPeoplesNames.Contains(x.Value));
            result.Value.ShouldBe("One");
        }

        [Fact]
        private async Task Should_handle_non_materialized_enumerablequery_closure()
        {
            var data = new List<LookupItem>
            {
                new LookupItem { Key = "1", Value = "One" },
                new LookupItem { Key = "2", Value = "Two" },
                new LookupItem { Key = "3", Value = "Three" },
            };
            var filteredPeoplesNames = data.Where(x => x.Value.StartsWith("O")).Select(x => x.Value).AsQueryable();
            var result = await _queryable.FirstOrDefaultAsync(x => filteredPeoplesNames.Contains(x.Value));
            result.Value.ShouldBe("One");
        }

        [Fact]
        private async Task Should_handle_closure_with_string_property()
        {
            var lookupitem = new LookupItem { Key = "1", Value = "One" };
            var result = await _queryable.FirstOrDefaultAsync(x => x.Value == lookupitem.Value);
            result.Value.ShouldBe("One");
        }

        [Fact]
        private async Task Should_handle_string_closure()
        {
            var lookupitem = "One";
            var result = await _queryable.FirstOrDefaultAsync(x => x.Value == lookupitem);
            result.Value.ShouldBe("One");
        }
    }
}
