// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class EnumerableExtensions
    {
        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncRemoteQueryable<T> AsRemoteQueryable<T>(this IEnumerable<T> testData)
            => testData.CheckNotNull(nameof(testData)) is IAsyncRemoteQueryable<T> remoteQueryable
            ? remoteQueryable
            : new AsyncRemoteQueryable<T>(new TaskAsyncQueryProvider(), testData.AsQueryable().Expression);

        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteStreamQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncRemoteStreamQueryable<T> AsAsyncRemoteStreamQueryable<T>(this IEnumerable<T> testData)
        {
            if (testData.CheckNotNull(nameof(testData)) is IAsyncRemoteStreamQueryable<T> asyncRemoteStream)
            {
                return asyncRemoteStream;
            }

            var provider = CreateAsyncRemoteSreamProviderWithTestData<T, object?>(testData);
            return (IAsyncRemoteStreamQueryable<T>)Remote.Linq.RemoteQueryable.Factory.CreateQueryable<T>(provider);
        }

        private static Func<Expression, IAsyncEnumerable<TResult>> CreateAsyncRemoteSreamProviderWithTestData<TData, TResult>(IEnumerable<TData> items)
            where TData : TResult
        {
            static async IAsyncEnumerable<TResult> CreateAsyncStream(IEnumerable<TData> source)
            {
                foreach (var item in source)
                {
                    yield return await Task.FromResult(item).ConfigureAwait(false);
                }
            }

            return expression =>
            {
                var result = expression.Execute<IEnumerable<TData>>(_ => items.AsQueryable());
                return CreateAsyncStream(result);
            };
        }
    }
}
