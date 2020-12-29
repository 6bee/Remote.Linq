// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        public static IRemoteQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        public static IRemoteQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        public static IRemoteQueryable CreateEntityFrameworkQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        public static IRemoteQueryable<T> CreateEntityFrameworkQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        public static IRemoteQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, mapper, canBeEvaluatedLocally);

        public static IRemoteQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);

        public static IRemoteQueryable CreateEntityFrameworkQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        public static IAsyncRemoteQueryable<T> CreateEntityFrameworkQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable CreateEntityFrameworkAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable CreateEntityFrameworkAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable CreateEntityFrameworkAsyncQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");

        [Obsolete("Method was renamed to 'CreateEntityFrameworkQueryable'.", true)]
        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotImplementedException("Method was renamed to 'CreateEntityFrameworkQueryable'.");
    }
}
