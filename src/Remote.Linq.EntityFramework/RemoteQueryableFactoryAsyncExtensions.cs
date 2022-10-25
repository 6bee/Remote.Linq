// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncExtensions
    {
        public static IAsyncRemoteQueryable CreateEntityFrameworkAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IAsyncRemoteQueryable CreateEntityFrameworkAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<object>? resultMapper = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<object>? resultMapper = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IAsyncRemoteQueryable CreateEntityFrameworkAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkAsyncQueryable<TSource>(factory, elementType, dataProvider, default(ITypeInfoProvider?), default(Func<SystemLinq.Expression, bool>?), resultMapper);

        public static IAsyncRemoteQueryable CreateEntityFrameworkAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, ValueTask<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => factory.CreateAsyncQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkAsyncQueryable<T, TSource>(factory, dataProvider, default(ITypeInfoProvider?), default(Func<SystemLinq.Expression, bool>?), resultMapper);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, ValueTask<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => factory.CreateAsyncQueryable<T, TSource>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);
    }
}