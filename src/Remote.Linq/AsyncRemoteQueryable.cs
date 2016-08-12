// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class AsyncRemoteQueryable
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return Create<IEnumerable<DynamicObject>>(elementType, dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return Create<T, IEnumerable<DynamicObject>>(dataProvider, resultMapper, typeResolver);
        }


        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, ITypeResolver typeResolver = null, IAsyncQueryResultMapper<object> resultMapper = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new AsyncObjectResultCaster();
            }

            return Create<object>(elementType, dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}"/> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, Task<object>> dataProvider, ITypeResolver typeResolver = null, IAsyncQueryResultMapper<object> resultMapper = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new AsyncObjectResultCaster();
            }

            return Create<T, object>(dataProvider, resultMapper, typeResolver);
        }


        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable Create<TSource>(Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable<T> Create<T, TSource>(Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper);
            return new Remote.Linq.DynamicQuery.AsyncRemoteQueryable<T>(queryProvider);
        }
    }
}
