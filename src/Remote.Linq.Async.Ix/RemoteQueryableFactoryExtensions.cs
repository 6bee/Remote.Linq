// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Async.Ix.DynamicQuery;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<DynamicObject?>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, DynamicObject?>(factory, dataProvider, new DynamicItemMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<object?>> dataProvider, IQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, object?>(factory, dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, IAsyncEnumerable<TSource>> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, TSource>(factory, (RemoteLinq.Expression exp, CancellationToken _) => dataProvider(exp), resultMapper, typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<DynamicObject?>> dataProvider, IDynamicObjectMapper? mapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, DynamicObject?>(factory, dataProvider, new DynamicItemMapper(mapper), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<object?>> dataProvider, IQueryResultMapper<object?>? resultMapper = null, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, object?>(factory, dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IAsyncQueryable{T}" /> that utilizes the async stream provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IAsyncQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IAsyncQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteLinqAsyncQueryProvider<TSource>(dataProvider, typeInfoProvider, canBeEvaluatedLocally, resultMapper);
            return new RemoteLinqAsyncQueryable<T>(queryProvider);
        }
    }
}
