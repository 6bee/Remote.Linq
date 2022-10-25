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
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateAsyncStreamQueryable<T, DynamicObject>(factory, dataProvider, context, resultMapper ?? new AsyncDynamicStreamResultMapper());

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateAsyncStreamQueryable<T, DynamicObject>(factory, dataProvider, context, resultMapper ?? new AsyncDynamicStreamResultMapper());

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null,
            IAsyncQueryResultMapper<DynamicObject>? resultMapper = null)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<object>? resultMapper = null)
            => CreateAsyncStreamQueryable<T, object>(factory, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<object>? resultMapper)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider,
            IExpressionToRemoteLinqContext? context = null,
            IAsyncQueryResultMapper<object>? resultMapper = null)
            => CreateAsyncStreamQueryable<T, object>(factory, dataProvider, context, resultMapper ?? new AsyncObjectResultCaster());

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<object>? resultMapper)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<object>? resultMapper)
            => CreateAsyncStreamQueryable<T>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, (RemoteLinq.Expression exp, CancellationToken _) => dataProvider(exp), context, resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, IAsyncEnumerable<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, default(IExpressionToRemoteLinqContext?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IAsyncQueryResultMapper<TSource> resultMapper)
        {
            var queryProvider = new AsyncRemoteStreamProvider<TSource>(dataProvider, context, resultMapper);
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
            ITypeInfoProvider? typeInfoProvider,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, typeInfoProvider, default(Func<SystemLinq.Expression, bool>?), resultMapper);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncRemoteStreamQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncRemoteStreamQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncRemoteStreamQueryable<T> CreateAsyncStreamQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally,
            IAsyncQueryResultMapper<TSource> resultMapper)
            => CreateAsyncStreamQueryable<T, TSource>(factory, dataProvider, RemoteQueryable.GetExpressionTranslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally), resultMapper);
    }
}