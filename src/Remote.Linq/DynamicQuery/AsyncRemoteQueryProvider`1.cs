// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.TypeExtensions;
using Aqua.TypeSystem;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using MethodInfo = System.Reflection.MethodInfo;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
public sealed class AsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
{
    private static readonly MethodInfo _executeMethod = typeof(AsyncRemoteQueryProvider<TSource>)
        .GetMethodEx(nameof(Execute), [typeof(MethodInfos.TResult)], typeof(SystemLinq.Expression));

    private readonly Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> _asyncDataProvider;
    private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
    private readonly IExpressionToRemoteLinqContext _context;

    public AsyncRemoteQueryProvider(
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> asyncDataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        _asyncDataProvider = asyncDataProvider.CheckNotNull();
        _resultMapper = resultMapper.CheckNotNull();
        _context = context ?? ExpressionTranslatorContext.Default;
    }

    /// <inheritdoc/>
    public IQueryable<TElement> CreateQuery<TElement>(SystemLinq.Expression expression)
        => new AsyncRemoteQueryable<TElement>(this, expression);

    /// <inheritdoc/>
    public IQueryable CreateQuery(SystemLinq.Expression expression)
    {
        var elementType = TypeHelper.GetElementType(expression.CheckNotNull().Type)
            ?? throw new RemoteLinqException($"Failed to get element type of {expression.Type}");
        return new RemoteQueryable(elementType, this, expression);
    }

    /// <inheritdoc/>
    public TResult Execute<TResult>(SystemLinq.Expression expression)
    {
        try
        {
            var task = ExecuteAsync<TResult>(expression, CancellationToken.None).AsTask();
            return task.GetAwaiter().GetResult();
        }
        catch (AggregateException ex)
        {
            if (ex.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            throw;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<TResult> ExecuteAsync<TResult>(SystemLinq.Expression expression, CancellationToken cancellation)
    {
        ExpressionHelper.CheckExpressionResultType<TResult>(expression);

        var rlinq = _context.ExpressionTranslator.TranslateExpression(expression);

        var dataRecords = await _asyncDataProvider(rlinq, cancellation).ConfigureAwait(false);

        var result = await _resultMapper.MapResultAsync<TResult>(dataRecords, expression, cancellation).ConfigureAwait(false);

        return result;
    }

    /// <inheritdoc/>
    public object? Execute(SystemLinq.Expression expression)
        => this.InvokeAndUnwrap<object?>(_executeMethod, expression);
}