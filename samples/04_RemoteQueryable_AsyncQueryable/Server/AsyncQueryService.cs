// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Aqua.Dynamic;
using Aqua.TypeExtensions;
using Remote.Linq.Async.Queryable.Expressions;
using Remote.Linq.Expressions;
using System.Reflection;

public class AsyncQueryService
{
    private Func<Type, IAsyncQueryable> AsyncQueryableByTypeProvider => new AsyncQueryableByTypeProviderWrapper(InMemoryDataStore.Instance.QueryableByTypeProvider).GetAsyncQueryableByType;

    public ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
        => queryExpression.ExecuteAsync(AsyncQueryableByTypeProvider, cancellation: cancellation);

    public IAsyncEnumerable<DynamicObject> ExecuteAsyncStreamQuery(Expression queryExpression, CancellationToken cancellation)
        => queryExpression.ExecuteAsyncStream(AsyncQueryableByTypeProvider, cancellation: cancellation);

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