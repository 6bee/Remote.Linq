// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        private const string TaskBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to provide ValueTask<> based data provider.";
        private const string EnumerableOfDynamicObjectBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to create data provider returning DynamicObject rather than IEnumerable<DynamicObject>.";

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(TaskBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(TaskBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(EnumerableOfDynamicObjectBasedMethodObsolete, true)]
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<IEnumerable<DynamicObject>>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => throw new NotSupportedException(EnumerableOfDynamicObjectBasedMethodObsolete);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, DynamicObject> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<DynamicObject>(elementType, dataProvider, new DynamicResultMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, DynamicObject> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, DynamicObject>(dataProvider, new DynamicResultMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<object>(elementType, dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, object> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, object>(dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return factory.CreateQueryable<DynamicObject>(elementType, dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<DynamicObject>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IDynamicObjectMapper? mapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return factory.CreateQueryable<T, DynamicObject>(dataProvider, resultMapper, typeInfoProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<object>(elementType, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<object>> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IAsyncQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, object>(dataProvider, resultMapper ?? new AsyncObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<TSource>(elementType, (expression, _) => dataProvider(expression), resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>((expression, _) => dataProvider(expression), resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.AsyncRemoteQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject?>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, DynamicObject?>(factory, dataProvider, new DynamicItemMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider, IAsyncQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, object?>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, TSource>(factory, (RemoteLinq.Expression exp, CancellationToken _) => dataProvider(exp), resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject?>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, DynamicObject?>(factory, dataProvider, new DynamicItemMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider, IAsyncQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, object?>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new AsyncRemoteStreamProvider<TSource>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);
            return new Remote.Linq.DynamicQuery.AsyncRemoteStreamQueryable<T>(queryProvider);
        }
    }
}
