// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.ComponentModel;
    using static Remote.Linq.EntityFrameworkCore.Helper;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateQueryable(elementType, dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateEntityFrameworkCoreQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateQueryable<T>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateEntityFrameworkCoreQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IQueryResultMapper<object>? resultMapper = null)
            => factory.CreateQueryable(elementType, dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<object>? resultMapper = null)
            => factory.CreateQueryable(elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IQueryResultMapper<object>? resultMapper = null)
            => factory.CreateQueryable<T>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<object>? resultMapper = null)
            => CreateEntityFrameworkCoreQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreQueryable<TSource>(factory, elementType, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IQueryResultMapper<TSource> resultMapper)
            => factory.CreateQueryable(elementType, dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IQueryResultMapper<TSource> resultMapper)
            => factory.CreateQueryable<T, TSource>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreQueryable<T, TSource>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
    }
}