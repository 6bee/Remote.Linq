// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.Dynamic;
using Remote.Linq;
using Remote.Linq.DynamicQuery;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

public sealed class When_executing_expression_extension_overloads : IDisposable
{
    private readonly TestDbContext _context = new();
    private readonly IRemoteQueryable<LookupItem> _remoteQueryable;

    public When_executing_expression_extension_overloads()
    {
        _context.Lookup.AddRange(
            new LookupItem { Key = "1", Value = "One" },
            new LookupItem { Key = "2", Value = "Two" },
            new LookupItem { Key = "3", Value = "Three" });
        _context.SaveChanges();

        Func<RemoteLinq.Expression, DynamicObject> dataProvider = expression => expression.ExecuteWithEntityFrameworkCore(_context);
        _remoteQueryable = Remote.Linq.RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<LookupItem>(dataProvider, (IExpressionToRemoteLinqContext)null);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public void Should_execute_mapped_results_with_database_context_and_queryable_providers()
    {
        var remoteExpression = CreateRemoteExpression(_remoteQueryable.Where(x => x.Key != "1").Expression);

        AssertMappedLookupItems(remoteExpression.ExecuteWithEntityFrameworkCore(_context));
        AssertMappedLookupItems(remoteExpression.ExecuteWithEntityFrameworkCore(QueryableProvider));
    }

    [Fact]
    public void Should_execute_typed_scalar_results_with_both_provider_forms()
    {
        var remoteExpression = CreateRemoteExpression(CountExpression());

        remoteExpression.ExecuteWithEntityFrameworkCore<int>(_context).ShouldBe(3);
        remoteExpression.ExecuteWithEntityFrameworkCore<int>(QueryableProvider).ShouldBe(3);
    }

    [Fact]
    public async Task Should_execute_async_mapped_and_typed_results_with_both_provider_forms()
    {
        var remoteExpression = CreateRemoteExpression(_remoteQueryable.Where(x => x.Key != "1").Expression);

        AssertMappedLookupItems(await remoteExpression.ExecuteWithEntityFrameworkCoreAsync(_context, TestContext.Current.CancellationToken));
        AssertMappedLookupItems(await remoteExpression.ExecuteWithEntityFrameworkCoreAsync(QueryableProvider, TestContext.Current.CancellationToken));

        var scalarRemoteExpression = CreateRemoteExpression(CountExpression());
        (await scalarRemoteExpression.ExecuteWithEntityFrameworkCoreAsync<int>(_context, TestContext.Current.CancellationToken)).ShouldBe(3);
        (await scalarRemoteExpression.ExecuteWithEntityFrameworkCoreAsync<int>(QueryableProvider, TestContext.Current.CancellationToken)).ShouldBe(3);
    }

    [Fact]
    public async Task Should_execute_async_streams_with_both_provider_forms()
    {
        var remoteExpression = CreateRemoteExpression(_remoteQueryable.Where(x => x.Key != "3").Expression);

        var dbContextItems = await ToListAsync(remoteExpression.ExecuteAsyncStreamWithEntityFrameworkCore(_context));
        dbContextItems.OfType<LookupItem>().Select(x => x.Key).OrderBy(x => x).ToArray().ShouldBe(new[] { "1", "2" });

        var providerItems = await ToListAsync(remoteExpression.ExecuteAsyncStreamWithEntityFrameworkCore(QueryableProvider));
        providerItems.Select(x => (string)x["Key"]).OrderBy(x => x).ToArray().ShouldBe(new[] { "1", "2" });
    }

    [Fact]
    public async Task Should_map_null_async_stream_item_to_default_dynamic_object()
    {
        var nullContext = new TestDbContext();
        nullContext.Lookup.Add(new LookupItem { Key = "1", Value = "One" });
        nullContext.SaveChanges();

        var source = nullContext.Lookup.Select(x => (LookupItem)null);
        var remoteExpression = CreateRemoteExpression(SystemLinq.Expression.Constant(source, typeof(IQueryable<LookupItem>)));
        Func<Type, IQueryable> provider = type => type == typeof(LookupItem) ? source : throw new NotSupportedException();

        var items = await ToListAsync(remoteExpression.ExecuteAsyncStreamWithEntityFrameworkCore(provider));

        var item = items.Single();
        item.ShouldBeOfType<DynamicObject>();
        item.IsNull.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_throw_operation_canceled_exception_when_canceled_before_execution()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var remoteExpression = CreateRemoteExpression(CountExpression());

        await Should.ThrowAsync<OperationCanceledException>(async () => await remoteExpression.ExecuteWithEntityFrameworkCoreAsync<int>(_context, cts.Token));
    }

    [Fact]
    public void Should_throw_argument_exception_for_non_queryable_async_stream_expression()
    {
        var remoteExpression = new RemoteLinq.ConstantExpression(42);

        var exception = Should.Throw<ArgumentException>(() => remoteExpression.ExecuteAsyncStreamWithEntityFrameworkCore(QueryableProvider));

        exception.Message.ShouldStartWith("Expression must be of type IQueryable<>");
        exception.ParamName.ShouldBe("expression");
    }

    [Fact]
    public async Task Should_convert_value_task_result_to_task_during_async_execution()
    {
        var remoteExpression = new RemoteLinq.ConstantExpression(ValueTask.FromResult(42), typeof(ValueTask<int>));

        var result = await remoteExpression.ExecuteWithEntityFrameworkCoreAsync<int>(_context, TestContext.Current.CancellationToken, ExpressionTranslatorContext.NoMappingContext);

        result.ShouldBe(42);
    }

    private Func<Type, IQueryable> QueryableProvider => type => type == typeof(LookupItem) ? _context.Lookup : throw new NotSupportedException();

    private static RemoteLinq.Expression CreateRemoteExpression(SystemLinq.Expression systemExpression)
        => new ExpressionTranslator(ExpressionTranslatorContext.NoMappingContext).TranslateExpression(systemExpression);

    private static void AssertMappedLookupItems(DynamicObject result)
    {
        result.ShouldNotBeNull();
        var items = (LookupItem[])ExpressionTranslatorContext.Default.ValueMapper.Map(result, typeof(LookupItem[]));
        items.Select(x => x.Key).OrderBy(x => x).ToArray().ShouldBe(new[] { "2", "3" });
    }

    private SystemLinq.Expression CountExpression()
    {
        var countMethod = typeof(Queryable).GetMethods().Single(x => x.Name == nameof(Queryable.Count) && x.IsGenericMethodDefinition && x.GetParameters().Length == 1)
            .MakeGenericMethod(typeof(LookupItem));

        return SystemLinq.Expression.Call(countMethod, SystemLinq.Expression.Constant(_remoteQueryable, typeof(IQueryable<LookupItem>)));
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
}
