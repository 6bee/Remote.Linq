// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

using Aqua.TypeExtensions;
using Remote.Linq.DynamicQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using MethodInfo = System.Reflection.MethodInfo;
using RemoteLinq = Remote.Linq.Expressions;

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
using IAsyncQueryProvider = Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider;
#else
using IAsyncQueryProvider = Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider;
#endif

/// <summary>
/// Represents a query provider for <i>Remote.Linq</i> version of asynchronous queryable sequences.
/// </summary>
public sealed class RemoteLinqEfCoreAsyncQueryProvider<TSource> : IRemoteLinqEfCoreAsyncQueryProvider
{
    private static readonly MethodInfo _executeMethod = typeof(RemoteLinqEfCoreAsyncQueryProvider<TSource>)
        .GetMethodEx(nameof(ExecuteAsyncInternal), [typeof(object)], typeof(Expression), typeof(CancellationToken));

    private static readonly MethodInfo _mapElementStreamToAsyncEnumerableMethodInfo =
        typeof(RemoteLinqEfCoreAsyncQueryProvider<TSource>).GetMethodEx(nameof(MapElementStreamToAsyncEnumerableInternal));

    private static readonly MethodInfo _mapElementStreamMethodInfo =
        typeof(RemoteLinqEfCoreAsyncQueryProvider<TSource>).GetMethodEx(nameof(MapElementStreamInternal));

    private readonly Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>>? _asyncDataProvider;
    private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
    private readonly IExpressionToRemoteLinqContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteLinqEfCoreAsyncQueryProvider{TSource}"/> class.
    /// </summary>
    [SecuritySafeCritical]
    public RemoteLinqEfCoreAsyncQueryProvider(
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> asyncDataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        _asyncDataProvider = asyncDataProvider.CheckNotNull();
        _context = context ?? ExpressionTranslatorContext.Default;
        _resultMapper = resultMapper.CheckNotNull();
    }

    IQueryable IQueryProvider.CreateQuery(Expression expression)
        => RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(this, expression);

    IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        => new RemoteLinqEfCoreAsyncQueryable<TElement>(this, expression);

    object? IQueryProvider.Execute(Expression expression)
        => _executeMethod.MakeGenericMethod(typeof(IQueryable).IsAssignableFrom(expression.Type) ? typeof(object) : expression.Type).Invoke(this, [expression, CancellationToken.None]);

    TResult IQueryProvider.Execute<TResult>(Expression expression)
        => ExecuteAsyncInternal<TResult>(expression, CancellationToken.None);

    public TResult ExecuteAsyncInternal<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rlinq = _context.ExpressionTranslator.TranslateExpression(expression);

        cancellationToken.ThrowIfCancellationRequested();

        if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
        {
            var resultItemType = typeof(TResult).GenericTypeArguments[0];

            return MapElementStreamToAsyncEnumerable<TResult>(resultItemType, expression, rlinq, cancellationToken);
        }

        if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
        {
            var genericTaskArgument = typeof(TResult).GenericTypeArguments[0];

            if (genericTaskArgument.IsGenericType && genericTaskArgument.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
            {
                var resultItemType = genericTaskArgument.GenericTypeArguments[0];

                return WrapIntoTypedTask<TResult>(genericTaskArgument, MapElementStreamToAsyncEnumerable<object>(resultItemType, expression, rlinq, cancellationToken));
            }

            return MapElementStream<TResult>(genericTaskArgument, expression, rlinq, cancellationToken);
        }

        return MapElementStream<Task<TResult>>(typeof(TResult), expression, rlinq, cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        => ExecuteAsyncInternal<TResult>(expression, cancellationToken);

    async ValueTask<TResult> IAsyncRemoteQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
       => await ExecuteAsyncInternal<Task<TResult>>(expression, cancellationToken).ConfigureAwait(false);

    IAsyncEnumerable<T> IAsyncRemoteStreamProvider.ExecuteAsyncRemoteStream<T>(Expression expression, CancellationToken cancellationToken)
       => ExecuteAsyncInternal<IAsyncEnumerable<T>>(expression, cancellationToken);

    private TResult MapElementStreamToAsyncEnumerable<TResult>(Type elementType, Expression expression, RemoteLinq.Expression rlinq, CancellationToken cancellationToken)
    {
        ExpressionHelper.CheckExpressionResultType(elementType, expression);

        cancellationToken.ThrowIfCancellationRequested();

        var method = _mapElementStreamToAsyncEnumerableMethodInfo.MakeGenericMethod(elementType);
        var result = method.Invoke(null, [expression, rlinq, _asyncDataProvider, _resultMapper, cancellationToken]);
        return (TResult)result!;
    }

    private static async IAsyncEnumerable<T> MapElementStreamToAsyncEnumerableInternal<T>(Expression expression, RemoteLinq.Expression rlinq, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> asyncDataProvider, IAsyncQueryResultMapper<TSource?> resultMapper, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var transferRecords = await asyncDataProvider(rlinq, cancellationToken).ConfigureAwait(false);
        var mappedResult = await resultMapper.MapResultAsync<IEnumerable<T>>(transferRecords, expression, cancellationToken).ConfigureAwait(false);

        foreach (var item in mappedResult)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    private TResult MapElementStream<TResult>(Type mapResultType, Expression expression, RemoteLinq.Expression rlinq, CancellationToken cancellationToken)
    {
        ExpressionHelper.CheckExpressionResultType(mapResultType, expression);

        cancellationToken.ThrowIfCancellationRequested();

        var method = _mapElementStreamMethodInfo.MakeGenericMethod(mapResultType);
        var result = method.Invoke(null, [expression, rlinq, _asyncDataProvider, _resultMapper, cancellationToken]);
        return (TResult)result!;
    }

    private static async Task<T> MapElementStreamInternal<T>(Expression expression, RemoteLinq.Expression rlinq, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> asyncDataProvider, IAsyncQueryResultMapper<TSource?> resultMapper, CancellationToken cancellationToken)
    {
        var dataRecords = await asyncDataProvider(rlinq, cancellationToken).ConfigureAwait(false);

        var result = await resultMapper.MapResultAsync<T>(dataRecords, expression, cancellationToken).ConfigureAwait(false);

        return result;
    }

    private static TResult WrapIntoTypedTask<TResult>(Type innerType, object objectToWrap)
        => (TResult)RemoteLinqEfCoreAsyncQueryProvider.TaskFromResultMethodInfo.MakeGenericMethod(innerType).Invoke(null, [objectToWrap])!;
}