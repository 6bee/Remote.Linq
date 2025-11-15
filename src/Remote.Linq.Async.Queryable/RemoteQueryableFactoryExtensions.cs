// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable;

using Aqua.Dynamic;
using Remote.Linq.Async.Queryable.DynamicQuery;
using Remote.Linq.DynamicQuery;
using System.ComponentModel;
using RemoteLinq = Remote.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteQueryableFactoryExtensions
{
    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject?>>? asyncDataProvider,
        IExpressionToRemoteLinqContext? context = null)
        => CreateAsyncQueryable<T, DynamicObject>(factory, asyncStreamProvider, asyncDataProvider, new AsyncDynamicStreamResultMapper(context?.ValueMapper), context);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, IAsyncEnumerable<object?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, ValueTask<object?>>? asyncDataProvider,
        IAsyncQueryResultMapper<object>? resultMapper = null,
        IExpressionToRemoteLinqContext? context = null)
        => CreateAsyncQueryable<T, object>(factory, asyncStreamProvider, asyncDataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, ValueTask<TSource?>>? asyncDataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper,
        IExpressionToRemoteLinqContext? context = null)
        => CreateAsyncQueryable<T, TSource>(
            factory,
            asyncStreamProvider is null ? null : (RemoteLinq.Expression exp, CancellationToken _) => asyncStreamProvider(exp),
            asyncDataProvider is null ? null : (RemoteLinq.Expression exp, CancellationToken _) => asyncDataProvider(exp),
            resultMapper,
            context);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject?>>? asyncDataProvider,
        IExpressionToRemoteLinqContext? context = null)
        => CreateAsyncQueryable<T, DynamicObject>(factory, asyncStreamProvider, asyncDataProvider, new AsyncDynamicStreamResultMapper(context?.ValueMapper), context);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>>? asyncDataProvider,
        IAsyncQueryResultMapper<object>? resultMapper = null,
        IExpressionToRemoteLinqContext? context = null)
        => CreateAsyncQueryable<T, object>(factory, asyncStreamProvider, asyncDataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>>? asyncStreamProvider,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>>? asyncDataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper,
        IExpressionToRemoteLinqContext? context = null)
    {
        var queryProvider = new RemoteLinqAsyncQueryProvider<TSource>(asyncStreamProvider, asyncDataProvider, resultMapper, context);
        return new RemoteLinqAsyncQueryable<T>(queryProvider);
    }
}