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
    public static class RemoteQueryableFactoryAsyncStreamExtensions
    {
        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, mapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider, IAsyncQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateAsyncStreamQueryable<T>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreAsyncStreamQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateAsyncStreamQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));
    }
}