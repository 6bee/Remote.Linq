// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using System.ComponentModel;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteQueryableFactoryExtensions
{
    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IRemoteQueryable CreateQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, DynamicObject> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateQueryable<DynamicObject>(factory, elementType, dataProvider, context, resultMapper ?? new DynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IRemoteQueryable CreateQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, DynamicObject> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, DynamicObject> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateQueryable<T, DynamicObject>(factory, dataProvider, context, resultMapper ?? new DynamicResultMapper());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, DynamicObject> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IRemoteQueryable CreateQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, object?> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IQueryResultMapper<object>? resultMapper = null)
        => CreateQueryable<object>(factory, elementType, dataProvider, context, resultMapper ?? new ObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    public static IRemoteQueryable CreateQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, object?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IQueryResultMapper<object>? resultMapper = null)
        => CreateQueryable(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, object?> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IQueryResultMapper<object>? resultMapper = null)
        => CreateQueryable<T, object>(factory, dataProvider, context, resultMapper ?? new ObjectResultCaster());

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, object?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IQueryResultMapper<object>? resultMapper = null)
        => CreateQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable CreateQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable CreateQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
        return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
    }

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable CreateQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<TSource>(factory, elementType, dataProvider, typeInfoProvider, null, resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable CreateQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<TSource>(factory, elementType, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
        return new RemoteQueryable<T>(queryProvider);
    }

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<T, TSource>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

    /// <summary>
    /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
    /// </summary>
    /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
    /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
    public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, TSource?> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IQueryResultMapper<TSource> resultMapper)
        => CreateQueryable<T, TSource>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
}