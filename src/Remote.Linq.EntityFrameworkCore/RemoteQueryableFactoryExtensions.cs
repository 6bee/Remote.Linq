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
    public static class RemoteQueryableFactoryExtensions
    {
        private const string EnumerableOfDynamicObjectBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to create data provider returning DynamicObject rather than IEnumerable<DynamicObject>.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable(elementType, dataProvider, GetOrCreateContext(context));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable<T>(dataProvider, GetOrCreateContext(context));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object> dataProvider,
            IQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, GetOrCreateContext(context));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            IQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object> dataProvider,
            IQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable<T>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            IQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreQueryable<T>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, GetOrCreateContext(context));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreQueryable(factory, elementType, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreQueryable<T, TSource>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));
    }
}