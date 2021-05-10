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
        /// Creates an <see cref="IRemoteQueryable{T}"/> for given test data.
        /// </summary>
        public static IRemoteQueryable<T> AsRemoteQueryable<T>(this IEnumerable<T> testData, IExpressionTranslatorContext? context = null)
            => testData.CheckNotNull(nameof(testData)) is IRemoteQueryable<T> remoteQueryable
            ? remoteQueryable
            : testData.AsAsyncRemoteQueryable(context);

        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncRemoteQueryable<T> AsAsyncRemoteQueryable<T>(this IEnumerable<T> testData, IExpressionTranslatorContext? context = null)
            => testData.CheckNotNull(nameof(testData)) is IAsyncRemoteQueryable<T> remoteQueryable
            ? remoteQueryable
            : new AsyncRemoteQueryable<T>(new TaskAsyncQueryProvider(context), testData.AsQueryable().Expression);

        /// <summary>
        /// !!! For unit testing only !!! <br />
        /// Creates an <see cref="IAsyncRemoteStreamQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncRemoteStreamQueryable<T> AsAsyncRemoteStreamQueryable<T>(this IEnumerable<T> testData, Action<Expression>? onExecuteQuery = null)
        {
            if (testData.CheckNotNull(nameof(testData)) is IAsyncRemoteStreamQueryable<T> asyncRemoteStream)
            {
                return asyncRemoteStream;
            }

            Func<Expression, IAsyncEnumerable<object?>> provider = (Expression expression) =>
            {
                onExecuteQuery?.Invoke(expression);

                var result = expression.Execute<IEnumerable<T>>(_ => testData.AsQueryable());
                return CreateAsyncStream(result);
            };

            static async IAsyncEnumerable<object?> CreateAsyncStream(IEnumerable<T> source)
            {
                foreach (var item in source)
                {
                    yield return await new ValueTask<object?>(item).ConfigureAwait(false);
                }
            }

            return Remote.Linq.RemoteQueryable.Factory.CreateAsyncStreamQueryable<T>(provider);
        }
    }
}