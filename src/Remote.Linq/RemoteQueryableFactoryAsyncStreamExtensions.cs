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
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryAsyncStreamExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncStreamQueryable<T, DynamicObject>(factory, dataProvider, new DynamicItemMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, mapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncStreamQueryable<T, DynamicObject>(factory, dataProvider, new DynamicItemMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> dataProvider,
            IDynamicObjectMapper? mapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, mapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncStreamQueryable<T, object>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncStreamQueryable<T, object>(factory, dataProvider, resultMapper ?? new AsyncObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider,
            IAsyncQueryResultMapper<object>? resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
            => CreateAsyncStreamQueryable<T, TSource>(factory, (RemoteLinq.Expression exp, CancellationToken _) => dataProvider(exp), resultMapper, context);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            var queryProvider = new AsyncRemoteStreamProvider<TSource>(dataProvider, resultMapper, context);
            return new AsyncRemoteStreamQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }
}