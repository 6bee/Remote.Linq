// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.TestSupport
{
    using Remote.Linq.Async.Ix.Expressions;
    ////using Remote.Linq.Expressions;
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
        {
            if (testData.CheckNotNull(nameof(testData)) is IAsyncQueryable<T> asyncQueryable)
            {
                return asyncQueryable;
            }

            var provider = CreateAsyncQueryableProviderWithTestData<T, object?>(testData);
            return RemoteQueryable.Factory.CreateQueryable<T>(provider);
        }

        private static Func<Expression, IAsyncEnumerable<TResult>> CreateAsyncQueryableProviderWithTestData<TData, TResult>(IEnumerable<TData> items)
            where TData : TResult
        {
            return expression =>
            {
                var result = expression.Execute<IAsyncEnumerable<TData>>(_ => AsyncStreamProvider(items));
                return AsyncItemMapper(result);
            };

            static async IAsyncEnumerable<TData> AsyncStreamProvider(IEnumerable<TData> source)
            {
                foreach (var item in source)
                {
                    yield return await new ValueTask<TData>(item).ConfigureAwait(false);
                }
            }

            static async IAsyncEnumerable<TResult> AsyncItemMapper(IAsyncEnumerable<TData> source)
            {
                await foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
