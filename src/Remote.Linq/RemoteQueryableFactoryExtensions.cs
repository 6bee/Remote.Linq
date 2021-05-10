// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        private const string EnumerableOfDynamicObjectBasedMethodObsolete = "This method can no longer be used and will be removed in a future version. Make sure to create data provider returning DynamicObject rather than IEnumerable<DynamicObject>.";

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
        public static IRemoteQueryable CreateQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateQueryable<DynamicObject>(factory, elementType, dataProvider, new DynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable(factory, elementType, dataProvider, mapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            IDynamicObjectMapper? mapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateQueryable<T, DynamicObject>(factory, dataProvider, new DynamicResultMapper(mapper), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, DynamicObject> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IDynamicObjectMapper? mapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T>(factory, dataProvider, mapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object?> dataProvider,
            IQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateQueryable<object>(factory, elementType, dataProvider, resultMapper ?? new ObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IRemoteQueryable CreateQueryable(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, object?> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object?> dataProvider,
            IQueryResultMapper<object>? resultMapper = null,
            IExpressionToRemoteLinqContext? context = null)
            => CreateQueryable<T, object>(factory, dataProvider, resultMapper ?? new ObjectResultCaster(), context);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, object?> dataProvider,
            ITypeInfoProvider? typeInfoProvider,
            IQueryResultMapper<object>? resultMapper = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(
            this RemoteQueryableFactory factory,
            Type elementType,
            Func<RemoteLinq.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<TSource>(factory, elementType, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, resultMapper, context);
            return new RemoteQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T, TSource>(
            this RemoteQueryableFactory factory,
            Func<RemoteLinq.Expression, TSource?> dataProvider,
            IQueryResultMapper<TSource> resultMapper,
            ITypeInfoProvider? typeInfoProvider,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateQueryable<T, TSource>(factory, dataProvider, resultMapper, RemoteQueryable.GetExpressionTRanslatorContextOrNull(typeInfoProvider, canBeEvaluatedLocally));
    }
}