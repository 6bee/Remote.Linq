// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Remote.Linq.Async;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using Remote.Linq.Expressions;

/// <summary>
/// Verifies that the internal <c>AsyncScalarQueryExpressionReWriter</c> maps synchronous scalar <c>Queryable</c>
/// operations to their Entity Framework Core async equivalents (with an appended cancellation-token argument).
/// The rewriter is internal, so it is exercised through the public EF Core async remote queryable created by
/// <c>RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable</c>.
/// </summary>
public sealed class When_rewriting_scalar_queries_to_async : IDisposable
{
    private readonly TestDbContext _context;
    private readonly IQueryable<LookupItem> _queryable;

    public When_rewriting_scalar_queries_to_async()
    {
        _context = new TestDbContext();
        _context.Lookup.Add(new LookupItem { Key = "1", Value = "One" });
        _context.Lookup.Add(new LookupItem { Key = "2", Value = "Two" });
        _context.Lookup.Add(new LookupItem { Key = "3", Value = "Three" });
        _context.SaveChanges();

        _queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreAsyncQueryable<LookupItem>((x, c) => x.ExecuteWithEntityFrameworkCoreAsync(_context, c), context: null);
    }

    public void Dispose() => _context.Dispose();

    public static IEnumerable<object[]> Scalar_operations()
    {
        yield return new object[] { "All", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.AllAsync(x => x.Value.Length > 2, c)), true };
        yield return new object[] { "Any", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.AnyAsync(x => x.Key == "2", c)), true };
        yield return new object[] { "Any_no_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.AnyAsync(c)), true };
        yield return new object[] { "Contains", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.Select(x => x.Key).ContainsAsync("2", c)), true };
        yield return new object[] { "Contains_no_match", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.Select(x => x.Key).ContainsAsync("9", c)), false };
        yield return new object[] { "Count", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.CountAsync(c)), 3 };
        yield return new object[] { "Count_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.CountAsync(x => x.Key != "1", c)), 2 };
        yield return new object[] { "LongCount", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.LongCountAsync(c)), 3L };
        yield return new object[] { "LongCount_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.LongCountAsync(x => x.Key != "1", c)), 2L };
        yield return new object[] { "First", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.FirstAsync(c)), "1" };
        yield return new object[] { "First_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.FirstAsync(x => x.Key == "2", c)), "2" };
        yield return new object[] { "FirstOrDefault", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.FirstOrDefaultAsync(c)), "1" };
        yield return new object[] { "FirstOrDefault_no_match", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.FirstOrDefaultAsync(x => x.Key == "9", c)), null };
        yield return new object[] { "Last", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.LastAsync(c)), "3" };
        yield return new object[] { "Last_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.LastAsync(x => x.Key != "3", c)), "2" };
        yield return new object[] { "LastOrDefault_no_match", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.LastOrDefaultAsync(x => x.Key == "9", c)), null };
        yield return new object[] { "Single_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.SingleAsync(x => x.Key == "2", c)), "2" };
        yield return new object[] { "Single_no_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.Where(x => x.Key == "2").SingleAsync(c)), "2" };
        yield return new object[] { "SingleOrDefault_with_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.SingleOrDefaultAsync(x => x.Key == "9", c)), null };
        yield return new object[] { "SingleOrDefault_no_predicate", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.Where(x => x.Key == "9").SingleOrDefaultAsync(c)), null };
        yield return new object[] { "Min", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.MinAsync(x => x.Value.Length, c)), 3 };
        yield return new object[] { "Max", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.MaxAsync(x => x.Value.Length, c)), 5 };
        yield return new object[] { "Sum", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.SumAsync(x => x.Value.Length, c)), 11 };
        yield return new object[] { "Average", (Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>>)(async (q, c) => await q.AverageAsync(x => x.Value.Length, c)), 11.0 / 3 };
    }

    [Theory]
    [MemberData(nameof(Scalar_operations))]
    public async Task Should_execute_scalar_operation_as_entity_framework_core_async_equivalent(
        string operation,
        Func<IQueryable<LookupItem>, CancellationToken, ValueTask<object>> operationFunc,
        object expected)
    {
        var result = await operationFunc(_queryable, TestContext.Current.CancellationToken);

        if (result is LookupItem item)
        {
            item.Key.ShouldBe(expected, $"for operation {operation}");
        }
        else
        {
            result.ShouldBe(expected, $"for operation {operation}");
        }
    }

    [Fact]
    public async Task Should_verify_appended_cancellation_token_argument_when_token_is_canceled()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => _queryable.CountAsync(cancellation.Token).AsTask());
    }

    [Fact]
    public async Task Should_execute_constant_expression_unchanged_through_scalar_rewriting()
    {
        var expression = new ConstantExpression(42);

        var result = await expression.ExecuteWithEntityFrameworkCoreAsync<int>(_context, TestContext.Current.CancellationToken);

        result.ShouldBe(42);
    }
}
