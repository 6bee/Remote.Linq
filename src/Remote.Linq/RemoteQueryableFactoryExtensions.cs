// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.Dynamic;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RemoteQueryableFactoryExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return factory.CreateQueryable<IEnumerable<DynamicObject>>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        public static IQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new DynamicResultMapper(mapper);
            return factory.CreateQueryable<T, IEnumerable<DynamicObject>>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable CreateQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return factory.CreateQueryable<object>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        public static IQueryable<T> CreateQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new ObjectResultCaster();
            }

            return factory.CreateQueryable<T, object>(dataProvider, resultMapper ?? new ObjectResultCaster(), queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IQueryable CreateQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IQueryable<T> CreateQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new RemoteQueryProvider<TSource>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable<T>(queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable CreateAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return factory.CreateAsyncQueryable<IEnumerable<DynamicObject>>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        public static IQueryable<T> CreateAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var resultMapper = new AsyncDynamicResultMapper(mapper);
            return factory.CreateAsyncQueryable<T, IEnumerable<DynamicObject>>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        public static IQueryable CreateAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new AsyncObjectResultCaster();
            }

            return factory.CreateAsyncQueryable<object>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}"/> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        public static IQueryable<T> CreateAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            if (ReferenceEquals(null, resultMapper))
            {
                resultMapper = new AsyncObjectResultCaster();
            }

            return factory.CreateAsyncQueryable<T, object>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IQueryable CreateAsyncQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.RemoteQueryable(elementType, queryProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified.
        /// </summary>
        /// <typeparam name="T">Element type of the <see cref="IQueryable{T}"/>.</typeparam>
        /// <typeparam name="TSource">Data type served by the data provider.</typeparam>
        public static IQueryable<T> CreateAsyncQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var queryProvider = new AsyncRemoteQueryProvider<TSource>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);
            return new Remote.Linq.DynamicQuery.AsyncRemoteQueryable<T>(queryProvider);
        }
    }
}
