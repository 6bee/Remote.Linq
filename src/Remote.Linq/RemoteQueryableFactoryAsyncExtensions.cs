// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using System.ComponentModel;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteQueryableFactoryAsyncExtensions
{
    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<DynamicObject>(factory, elementType, dataProvider!, context, resultMapper ?? new AsyncDynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<DynamicObject>(factory, elementType, dataProvider!, context, resultMapper ?? new AsyncDynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<T, DynamicObject>(factory, dataProvider!, context, resultMapper ?? new AsyncDynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<T, DynamicObject>(factory, dataProvider!, context, resultMapper ?? new AsyncDynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateAsyncQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<object>(factory, elementType, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<object>(factory, elementType, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IAsyncRemoteQueryable CreateAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<T, object>(factory, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<T, object>(factory, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateAsyncQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        dataProvider.AssertNotNull();
        return CreateAsyncQueryable<TSource>(factory, elementType, (expression, _) => dataProvider(expression), context, resultMapper);
    }

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, context, resultMapper);
        return new AsyncRemoteQueryable(elementType, queryProvider);
    }

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable CreateAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<TSource>(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        dataProvider.AssertNotNull();
        return factory.CreateAsyncQueryable<T, TSource>((expression, _) => dataProvider(expression), context, resultMapper);
    }

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<T, TSource>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<T, TSource>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, context, resultMapper);
        return new AsyncRemoteQueryable<T>(queryProvider);
    }

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<T, TSource>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IAsyncRemoteQueryable<T> CreateAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateAsyncQueryable<T, TSource>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
}