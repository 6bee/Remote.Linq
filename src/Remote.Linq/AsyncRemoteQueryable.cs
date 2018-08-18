// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AsyncRemoteQueryable
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable(elementType, dataProvider, typeResolver, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable<T>(dataProvider, typeResolver, mapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable() instead.", false)]
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, ITypeResolver typeResolver = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable(elementType, dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable<T>() instead.", false)]
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, Task<object>> dataProvider, ITypeResolver typeResolver = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable<T>(dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable<TSource>() instead.", false)]
        public static IQueryable Create<TSource>(Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable<TSource>(elementType, dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        [Obsolete("Please use Remote.Linq.RemoteQueryable.Factory.CreateAsyncQueryable<T, TSource>() instead.", false)]
        public static IQueryable<T> Create<T, TSource>(Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => RemoteQueryable.Factory.CreateAsyncQueryable<T, TSource>(dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
    }
}
