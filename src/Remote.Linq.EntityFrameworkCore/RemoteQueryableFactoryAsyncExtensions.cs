// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.DynamicQuery;
using Remote.Linq.EntityFrameworkCore.DynamicQuery;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using static Remote.Linq.EntityFrameworkCore.Helper;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class RemoteQueryableFactoryAsyncExtensions
{
    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<DynamicObject>(dataProvider!, GetOrCreateContext(context), resultMapper ?? new AsyncDynamicResultMapper());
        return RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(elementType, queryProvider, null);
    }

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, (expression, _) => dataProvider(expression), context, resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<DynamicObject>(dataProvider!, GetOrCreateContext(context), resultMapper ?? new AsyncDynamicResultMapper());
        return new RemoteLinqEfCoreAsyncQueryable<T>(queryProvider, null);
    }

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, (expression, _) => dataProvider(expression)!, context, resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<DynamicObject>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<object>(dataProvider!, GetOrCreateContext(context), resultMapper ?? new AsyncObjectResultCaster());
        return RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(elementType, queryProvider, null);
    }

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, (expression, _) => dataProvider(expression)!, context, resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<object>(dataProvider!, GetOrCreateContext(context), resultMapper ?? new AsyncObjectResultCaster());
        return new RemoteLinqEfCoreAsyncQueryable<T>(queryProvider, null);
    }

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        IExpressionToRemoteLinqContext? context = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, (expression, _) => dataProvider(expression), context, resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<object?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
        IAsyncQueryResultMapper<object>? resultMapper = null)
        => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<TSource>(dataProvider!, GetOrCreateContext(context), resultMapper);
        return RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(elementType, queryProvider, null);
    }

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<TSource>(factory, elementType, (expression, _) => dataProvider(expression), context, resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
        this RemoteQueryableFactory factory,
        Type elementType,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
    {
        var queryProvider = new RemoteLinqEfCoreAsyncQueryProvider<TSource>(dataProvider!, GetOrCreateContext(context), resultMapper);
        return new RemoteLinqEfCoreAsyncQueryable<T>(queryProvider, null);
    }

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        IExpressionToRemoteLinqContext? context,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, (expression, _) => dataProvider(expression), context, resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

    public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
        this RemoteQueryableFactory factory,
        Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> dataProvider,
        ITypeInfoProvider? typeInfoProvider,
        Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
        IAsyncQueryResultMapper<TSource> resultMapper)
        => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
}