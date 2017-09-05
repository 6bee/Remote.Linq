// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class RemoteQueryableFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return factory.Create<IEnumerable<DynamicObject>>(elementType, dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return factory.Create<T, IEnumerable<DynamicObject>>(dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        public static IQueryable Create(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return factory.Create<object>(elementType, dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        public static IQueryable<T> Create<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, ITypeResolver typeResolver = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return factory.Create<T, object>(dataProvider, resultMapper, typeResolver, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable Create<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/></typeparam>
        /// <typeparam name="TSource">Data type served by the data provider</typeparam>
        public static IQueryable<T> Create<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, typeResolver, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable<T>(queryProvider);
        }
    }
}
