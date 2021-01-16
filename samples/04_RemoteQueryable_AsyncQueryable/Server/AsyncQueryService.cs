// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Remote.Linq.Async.Queryable.Expressions;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class AsyncQueryService
    {
        private Func<Type, IAsyncQueryable> AsyncQueryableByTypeProvider => new AsyncQueryableByTypeProviderWrapper(InMemoryDataStore.Instance.QueryableByTypeProvider).GetAsyncQueryableByType;

        public IAsyncEnumerable<DynamicObject> ExecuteAsyncStreamQuery(Expression queryExpression, CancellationToken cancellation)
        {
            // DEMO: for demo purpose we fetch query result into memory
            // and use test-support extension method to return an asyn stream
            return queryExpression.ExecuteAsyncStream(AsyncQueryableByTypeProvider);
        }

        private sealed class AsyncQueryableByTypeProviderWrapper
        {
            private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

            private static readonly MethodInfo AsAsyncQueryableMethod = typeof(AsyncQueryableByTypeProviderWrapper).GetMethod(nameof(AsAsyncQueryable), PrivateStatic);

            private readonly Func<Type, IQueryable> _queryableByTypeProvider;

            public AsyncQueryableByTypeProviderWrapper(Func<Type, IQueryable> queryableByTypeProvider)
            {
                _queryableByTypeProvider = queryableByTypeProvider;
            }

            public IAsyncQueryable GetAsyncQueryableByType(Type type)
            {
                var queryable = _queryableByTypeProvider(type);
                if (queryable.GetType().Implements(typeof(IQueryable<>), out var genericArguments))
                {
                    return (IAsyncQueryable)AsAsyncQueryableMethod
                        .MakeGenericMethod(genericArguments)
                        .Invoke(null, new[] { queryable });
                }

                throw new NotSupportedException($"{type} is not supported");
            }

            private static IAsyncQueryable<T> AsAsyncQueryable<T>(IQueryable<T> queriable) => queriable.ToAsyncEnumerable().AsAsyncQueryable();
        }
    }
}
