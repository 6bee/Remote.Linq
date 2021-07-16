// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using static Remote.Linq.EntityFrameworkCore.Helper;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncStreamExtensions
    {
        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, GetOrCreateContext(context));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object?>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object?>? resultMapper = null,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncStreamQueryable<T, TSource>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));
    }
}