// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using static Remote.Linq.EntityFrameworkCore.ExpressionExecution.Helper;
    using static Remote.Linq.EntityFrameworkCore.Helper;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncStreamExtensions
    {
        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<object?>? resultMapper = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<object?>? resultMapper = null)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => factory.CreateAsyncStreamQueryable<T, TSource>(dataProvider, GetOrCreateContext(context), resultMapper);

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
    }
}