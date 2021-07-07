// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using static Remote.Linq.EntityFrameworkCore.Helper;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncExtensions
    {
        private const string TaskBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to call `" + nameof(CreateEntityFrameworkCoreAsyncQueryable) + "` and provide ValueTask<> based data provider.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider = null,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable<T>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable(factory, elementType, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => factory.CreateAsyncQueryable<T, TSource>(dataProvider, resultMapper, GetOrCreateContext(context));

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, ValueTask<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(factory, dataProvider, resultMapper, GetExpressionToRemoteLinqContext(typeInfoProvider, canBeEvaluatedLocally));
    }
}