// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
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
        public static IQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally);

        public static IQueryable CreateEntityFrameworkQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);

        public static IQueryable CreateEntityFrameworkQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<TSource>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);

        public static IQueryable CreateEntityFrameworkAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally);

        public static IQueryable CreateEntityFrameworkAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally);

        public static IQueryable CreateEntityFrameworkAsyncQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<TSource>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);

        public static IQueryable<T> CreateEntityFrameworkAsyncQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T, TSource>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally);
    }
}
