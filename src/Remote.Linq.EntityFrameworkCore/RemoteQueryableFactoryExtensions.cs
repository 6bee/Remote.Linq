// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
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

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, DynamicObject> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, DynamicObject> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));
    }
}