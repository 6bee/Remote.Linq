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
        public static IRemoteQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, object?> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<object>(elementType, dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, object?> dataProvider, ITypeInfoProvider? typeInfoProvider = null, IQueryResultMapper<object>? resultMapper = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, object>(dataProvider, resultMapper ?? new ObjectResultCaster(), typeInfoProvider, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<RemoteLinq.Expression, TSource?> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IRemoteQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IRemoteQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IRemoteQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<RemoteLinq.Expression, TSource?> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeInfoProvider? typeInfoProvider = null, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeInfoProvider, resultMapper, canBeEvaluatedLocally);
            return new RemoteQueryable<T>(queryProvider);
        }
    }
}