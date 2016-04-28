// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class RemoteQueryable
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return Create<IEnumerable<DynamicObject>>(elementType, dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return Create<T, IEnumerable<DynamicObject>>(dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return Create<object>(elementType, dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return Create<T, object>(dataProvider, resultMapper, typeResolver);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable Create<TSource>(Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable<T> Create<T, TSource>(Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper);
            return new Remote.Linq.DynamicQuery.RemoteQueryable<T>(queryProvider);
        }
    }
}
