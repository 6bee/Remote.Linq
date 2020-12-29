// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.TestSupport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Expression = Remote.Linq.Expressions.Expression;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> testData)
            where T : class
        {
            if (testData.CheckNotNull(nameof(testData)) is IAsyncQueryable<T> asyncQueryable)
            {
                return asyncQueryable;
            }

            var provider = CreateAsyncQueryableProviderWithTestData<T, object>(testData);
            return RemoteQueryable.Factory.CreateQueryable<T>(provider!);
        }

        private static Func<Expression, IAsyncEnumerable<TResult>> CreateAsyncQueryableProviderWithTestData<TData, TResult>(IEnumerable<TData> items)
            where TData : class, TResult
            where TResult : class
        {
            return ExecuteAsync;

            IAsyncEnumerable<TResult> ExecuteAsync(Expression expression)
            {
                var result = items.ToAsyncEnumerable();

                return MapItemTypeAsync(result);

                static async IAsyncEnumerable<TResult> MapItemTypeAsync(IAsyncEnumerable<TData> source)
                {
                    await foreach (var item in source.ConfigureAwait(false))
                    {
                        yield return item!;
                    }
                }
            }
        }
    }
}
