// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        private const string TaskBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to provide ValueTask<> based data provider.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<TSource>(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, ValueTask<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, ValueTask<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<TSource>(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IAsyncEnumerable<DynamicObject?>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, mapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IAsyncEnumerable<object?>> dataProvider, IQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IAsyncRemoteStreamQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IAsyncEnumerable<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));
    }
}