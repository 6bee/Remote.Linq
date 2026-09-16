// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.Dynamic;
using Remote.Linq.Async;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RemoteLinq = Remote.Linq.Expressions;

public sealed class When_using_entity_framework_core_factory_extensions : IDisposable
{
    private readonly TestDbContext _context = new();

    public When_using_entity_framework_core_factory_extensions()
    {
        _context.Lookup.AddRange(
            new LookupItem { Key = "1", Value = "One" },
            new LookupItem { Key = "2", Value = "Two" });
        _context.SaveChanges();
    }

    [Fact]
    public void Should_execute_sync_queries_created_by_ef_core_factory()
    {
        Func<RemoteLinq.Expression, DynamicObject> provider = expression => expression.ExecuteWithEntityFrameworkCore(_context);
        var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(provider, (IExpressionToRemoteLinqContext)null);

        queryable.Single(item => item.Key == "2").Value.ShouldBe("Two");
    }

    [Fact]
    public async Task Should_execute_stream_queries_created_by_ef_core_factory()
    {
        Func<RemoteLinq.Expression, IAsyncEnumerable<object>> provider = expression => expression.ExecuteAsyncStreamWithEntityFrameworkCore(_context);
        var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncStreamQueryable<LookupItem>(provider, (IExpressionToRemoteLinqContext)null);
        var result = await ToListAsync(queryable.Where(item => item.Key == "1").AsAsyncEnumerable(cancellation: TestContext.Current.CancellationToken));

        result.Single().Value.ShouldBe("One");
    }

    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> source)
    {
        var result = new List<T>();
        await foreach (var item in source)
        {
            result.Add(item);
        }

        return result;
    }

    public void Dispose() => _context.Dispose();
}
