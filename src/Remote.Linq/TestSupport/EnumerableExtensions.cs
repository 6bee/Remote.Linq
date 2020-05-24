// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Remote.Linq;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncRemoteQueryable<T> AsRemoteQueryable<T>(this IEnumerable<T> source)
            => source is null
            ? throw new ArgumentNullException(nameof(source))
            : source is IAsyncRemoteQueryable<T> remoteQueryable
            ? remoteQueryable
            : new AsyncRemoteQueryable<T>(new TaskAsyncQueryProvider(), source.AsQueryable().Expression);

#if ASYNC_STREAM
        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteStreamQueryable{T}"/> for given test data.
        /// </summary>
        public static IQueryable<T> AsAsyncRemoteStreamQueryable<T>(this IEnumerable<T> testData)
        {
            var provider = CreateAsyncRemoteSreamProviderWithTestData<T, object?>(testData);
            return Remote.Linq.RemoteQueryable.Factory.CreateQueryable<T>(provider);
        }

        private static Func<Expression, CancellationToken, IAsyncEnumerable<TResult>> CreateAsyncRemoteSreamProviderWithTestData<TData, TResult>(IEnumerable<TData> items)
            where TData : TResult
        {
            async IAsyncEnumerable<TResult> CreateAsyncStream(IEnumerable<TData> source)
            {
                foreach (var item in source)
                {
                    yield return await Task.FromResult(item).ConfigureAwait(false);
                }
            }

            return (expression, cancellation) =>
            {
                var result = expression.Execute<IEnumerable<TData>>(_ => items.AsQueryable());
                return CreateAsyncStream(result);
            };
        }
#endif // ASYNC_STREAM
    }
}
