// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
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
        public static IQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable CreateEntityFrameworkCoreQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable(elementType, dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, object> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable CreateEntityFrameworkCoreQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<TSource>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, TSource> dataProvider, IQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateQueryable<T, TSource>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable CreateEntityFrameworkCoreAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IDynamicObjectMapper mapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, queryableResourceProvider, mapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable CreateEntityFrameworkCoreAsyncQueryable(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable(elementType, dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<object>> dataProvider, IQueryableResourceDescriptorProvider queryableResourceProvider = null, IAsyncQueryResultMapper<object> resultMapper = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T>(dataProvider, queryableResourceProvider, resultMapper, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable CreateEntityFrameworkCoreAsyncQueryable<TSource>(this RemoteQueryableFactory factory, Type elementType, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<TSource>(elementType, dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));

        public static IQueryable<T> CreateEntityFrameworkCoreAsyncQueryable<T, TSource>(this RemoteQueryableFactory factory, Func<Expressions.Expression, Task<TSource>> dataProvider, IAsyncQueryResultMapper<TSource> resultMapper, IQueryableResourceDescriptorProvider queryableResourceProvider = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => factory.CreateAsyncQueryable<T, TSource>(dataProvider, resultMapper, queryableResourceProvider, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated));
    }
}
