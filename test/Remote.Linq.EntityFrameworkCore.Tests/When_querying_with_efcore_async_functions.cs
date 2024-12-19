// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Microsoft.EntityFrameworkCore;
using Remote.Linq.Async;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public sealed class When_querying_with_efcore_async_functions : IDisposable
{
    private readonly TestContext _context;
    private readonly IQueryable<LookupItem> _queryable;

    public When_querying_with_efcore_async_functions()
    {
        _context = new TestContext();
        _context.Lookup.Add(new LookupItem { Key = "1", Value = "One" });
        _context.Lookup.Add(new LookupItem { Key = "2", Value = "Two" });
        _context.Lookup.Add(new LookupItem { Key = "3", Value = "Three" });
        _context.SaveChanges();

        _queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>((x, c) => x.ExecuteWithEntityFrameworkCoreAsync(_context, c), context: null);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task Should_query_multiple()
    {
        var results = await EntityFrameworkQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("O")));
        var keys = results.Select(x => x.Key).ToArray();
        keys.ShouldContain("1");
        keys.ShouldContain("2");
        keys.Length.ShouldBe(2);

        results = await AsyncQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("O")));
        keys = results.Select(x => x.Key).ToArray();
        keys.ShouldContain("1");
        keys.ShouldContain("2");
        keys.Length.ShouldBe(2);

        results = _queryable.Where(x => x.Value.ToUpper().Contains("O")).ToArray();
        keys = results.Select(x => x.Key).ToArray();
        keys.ShouldContain("1");
        keys.ShouldContain("2");
        keys.Length.ShouldBe(2);

        var resultsList = new List<LookupItem>();
        await foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("O")).AsAsyncEnumerable())
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        keys = results.Select(x => x.Key).ToArray();
        keys.ShouldContain("1");
        keys.ShouldContain("2");
        keys.Length.ShouldBe(2);

        resultsList = new List<LookupItem>();
        foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("O")))
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        keys = results.Select(x => x.Key).ToArray();
        keys.ShouldContain("1");
        keys.ShouldContain("2");
        keys.Length.ShouldBe(2);
    }

    [Fact]
    public async Task Should_query_multiple_with_projection()
    {
        var results = await EntityFrameworkQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("O")).Select(x => x.Key));
        results.ShouldContain("1");
        results.ShouldContain("2");
        results.Length.ShouldBe(2);

        results = await AsyncQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("O")).Select(x => x.Key));
        results.ShouldContain("1");
        results.ShouldContain("2");
        results.Length.ShouldBe(2);

        results = _queryable.Where(x => x.Value.ToUpper().Contains("O")).Select(x => x.Key).ToArray();
        results.ShouldContain("1");
        results.ShouldContain("2");
        results.Length.ShouldBe(2);

        var resultsList = new List<string>();
        await foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("O")).Select(x => x.Key).AsAsyncEnumerable())
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        results.ShouldContain("1");
        results.ShouldContain("2");
        results.Length.ShouldBe(2);

        resultsList = new List<string>();
        foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("O")).Select(x => x.Key))
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        results.ShouldContain("1");
        results.ShouldContain("2");
        results.Length.ShouldBe(2);
    }

    [Fact]
    public async Task Should_query_multiple_with_empty_result()
    {
        var results = await EntityFrameworkQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("no match")));
        results.ShouldBeEmpty();

        results = await AsyncQueryableExtensions.ToArrayAsync(_queryable.Where(x => x.Value.ToUpper().Contains("no match")));
        results.ShouldBeEmpty();

        results = _queryable.Where(x => x.Value.ToUpper().Contains("no match")).ToArray();
        results.ShouldBeEmpty();

        var resultsList = new List<LookupItem>();
        await foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("no match")).AsAsyncEnumerable())
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        results.ShouldBeEmpty();

        resultsList = new List<LookupItem>();
        foreach (var result in _queryable.Where(x => x.Value.ToUpper().Contains("no match")))
        {
            resultsList.Add(result);
        }

        results = resultsList.ToArray();
        results.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_query_single()
    {
        var result = await EntityFrameworkQueryableExtensions.SingleAsync(_queryable, x => x.Value.ToUpper().Contains("W"));

        result.Key.ShouldBe("2");

        result = await EntityFrameworkQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value.ToUpper().Contains("W")));

        result.Key.ShouldBe("2");

        result = await AsyncQueryableExtensions.SingleAsync(_queryable, x => x.Value.ToUpper().Contains("W"));

        result.Key.ShouldBe("2");

        result = await AsyncQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value.ToUpper().Contains("W")));

        result.Key.ShouldBe("2");

        result = _queryable.Single(x => x.Value.ToUpper().Contains("W"));

        result.Key.ShouldBe("2");

        result = _queryable.Where(x => x.Value.ToUpper().Contains("W")).Single();

        result.Key.ShouldBe("2");
    }

    [Fact]
    public async Task Should_query_single_with_projection()
    {
        var result = await EntityFrameworkQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value.ToUpper().Contains("W")).Select(x => x.Key));

        result.ShouldBe("2");

        result = await AsyncQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value.ToUpper().Contains("W")).Select(x => x.Key));

        result.ShouldBe("2");

        result = _queryable.Where(x => x.Value.ToUpper().Contains("W")).Select(x => x.Key).Single();

        result.ShouldBe("2");
    }

    [Fact]
    public async Task Should_query_single_with_subquery_predicate()
    {
        var result = await EntityFrameworkQueryableExtensions.SingleAsync(_queryable, x => x.Value == _queryable.First().Value);

        result.Key.ShouldBe("1");

        result = await EntityFrameworkQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value == _queryable.First().Value));

        result.Key.ShouldBe("1");

        result = await AsyncQueryableExtensions.SingleAsync(_queryable, x => x.Value == _queryable.First().Value);

        result.Key.ShouldBe("1");

        result = await AsyncQueryableExtensions.SingleAsync(_queryable.Where(x => x.Value == _queryable.First().Value));

        result.Key.ShouldBe("1");

        result = _queryable.Single(x => x.Value == _queryable.First().Value);

        result.Key.ShouldBe("1");

        result = _queryable.Where(x => x.Value == _queryable.First().Value).Single();

        result.Key.ShouldBe("1");
    }

    [Fact]
    public async Task SingleOrDefault_with_predicate_should_return_null_if_no_match()
    {
        var result = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_queryable, x => x.Value.ToUpper().Contains("no match"));

        result.ShouldBeNull();

        result = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_queryable.Where(x => x.Value.ToUpper().Contains("no match")));

        result.ShouldBeNull();

        result = await AsyncQueryableExtensions.SingleOrDefaultAsync(_queryable, x => x.Value.ToUpper().Contains("no match"));

        result.ShouldBeNull();

        result = await AsyncQueryableExtensions.SingleOrDefaultAsync(_queryable.Where(x => x.Value.ToUpper().Contains("no match")));

        result.ShouldBeNull();

        result = _queryable.SingleOrDefault(x => x.Value.ToUpper().Contains("no match"));

        result.ShouldBeNull();

        result = _queryable.Where(x => x.Value.ToUpper().Contains("no match")).SingleOrDefault();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task Should_throw_when_calling_single_or_default_with_predicate_on_query_with_multiple_results()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        var ex = await Should.ThrowAsync<InvalidOperationException>(() => EntityFrameworkQueryableExtensions.SingleOrDefaultAsync(_queryable, x => x.Value.ToUpper().Contains("O")));
        ex.Message.ShouldBe("Sequence contains more than one matching element");

        ex = await Should.ThrowAsync<InvalidOperationException>(async () => await AsyncQueryableExtensions.SingleOrDefaultAsync(_queryable, x => x.Value.ToUpper().Contains("O")));
        ex.Message.ShouldBe("Sequence contains more than one matching element");

        ex = Should.Throw<InvalidOperationException>(() => _queryable.SingleOrDefault(x => x.Value.ToUpper().Contains("O")));
        ex.Message.ShouldBe("Sequence contains more than one matching element");
    }

    [Fact]
    public async Task Single_with_predicate_should_faile_if_no_match()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        var ex = await Should.ThrowAsync<InvalidOperationException>(() => EntityFrameworkQueryableExtensions.SingleAsync(_queryable, x => x.Value.ToUpper().Contains("no match")));
        ex.Message.ShouldBe("Sequence contains no matching element");

        ex = await Should.ThrowAsync<InvalidOperationException>(async () => await AsyncQueryableExtensions.SingleAsync(_queryable, x => x.Value.ToUpper().Contains("no match")));
        ex.Message.ShouldBe("Sequence contains no matching element");

        ex = Should.Throw<InvalidOperationException>(() => _queryable.Single(x => x.Value.ToUpper().Contains("no match")));
        ex.Message.ShouldBe("Sequence contains no matching element");
    }
}