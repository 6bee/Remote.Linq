// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.ComponentModel;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        public static IRemoteQueryable CreateEntityFrameworkQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<DynamicObject>? resultMapper = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, object?> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<object>? resultMapper = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, object?> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IQueryResultMapper<object>? resultMapper = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkQueryable<TSource>(factory, elementType, dataProvider, default(ITypeInfoProvider?), default(Func<SystemLinq.Expression, bool>?), resultMapper);

        public static IRemoteQueryable CreateEntityFrameworkQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<Expressions.Expression, TSource?> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IQueryResultMapper<TSource> resultMapper)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper)
            => CreateEntityFrameworkQueryable<T, TSource>(factory, dataProvider, default(ITypeInfoProvider?), default(Func<SystemLinq.Expression, bool>?), resultMapper);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<Expressions.Expression, TSource?> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IQueryResultMapper<TSource> resultMapper)
            => factory.CreateQueryable<T, TSource>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);
    }
}