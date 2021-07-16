// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.TestSupport
{
    using Aqua.Dynamic;
    using Remote.Linq.Async.Queryable.Expressions;
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
        /// <b>!!! For unit testing only !!!</b><br />
        /// Creates an <see cref="IAsyncQueryable{T}"/> for given test data.
        /// </summary>
        public static IAsyncQueryable<T> AsAsyncQueryable<T>(
            this IEnumerable<T> testData,
            bool createAsyncStreamProvider = true,
            bool createAsyncDataProvider = true,
            bool useDynamicObjectConversion = false,
            Action<Expression>? onExecuteAsyncStream = null,
            Action<Expression>? onExecuteAsyncQuery = null)
            where T : class
        {
            if (testData.CheckNotNull(nameof(testData)) is IAsyncQueryable<T> asyncQueryable)
            {
                return asyncQueryable;
            }

            if (useDynamicObjectConversion)
            {
                var asyncStreamProvider = createAsyncStreamProvider ? CreateDynamicObjectAsyncStreamProviderWithTestData(testData, onExecuteAsyncStream) : null;
                var asyncDataProvider = createAsyncDataProvider ? CreateDynamicObjectAsyncDataProviderWithTestData(testData, onExecuteAsyncQuery) : null;
                return RemoteQueryable.Factory.CreateAsyncQueryable<T>(asyncStreamProvider, asyncDataProvider);
            }
            else
            {
                var asyncStreamProvider = createAsyncStreamProvider ? CreateAsyncStreamProviderWithTestData<T, object>(testData, onExecuteAsyncStream) : null;
                var asyncDataProvider = createAsyncDataProvider ? CreateAsyncDataProviderWithTestData<T, object>(testData, onExecuteAsyncQuery) : null;

                return RemoteQueryable.Factory.CreateAsyncQueryable<T>(asyncStreamProvider, asyncDataProvider);
            }
        }

        private static Func<Expression, IAsyncEnumerable<DynamicObject?>> CreateDynamicObjectAsyncStreamProviderWithTestData<TData>(IEnumerable<TData> items, Action<Expression>? onExecuteAsyncStream)
        {
            return ExecuteAsyncStream;

            IAsyncEnumerable<DynamicObject?> ExecuteAsyncStream(Expression expression)
            {
                onExecuteAsyncStream?.Invoke(expression);

                var result = expression.ExecuteAsyncStream(_ => items.ToAsyncEnumerable().AsAsyncQueryable());
                return result;
            }
        }

        private static Func<Expression, ValueTask<DynamicObject?>> CreateDynamicObjectAsyncDataProviderWithTestData<TData>(IEnumerable<TData> items, Action<Expression>? onExecuteAsyncQuery)
        {
            return ExecuteAsyncQuery;

            async ValueTask<DynamicObject?> ExecuteAsyncQuery(Expression expression)
            {
                onExecuteAsyncQuery?.Invoke(expression);

                var result = await expression.ExecuteAsync(_ => items.ToAsyncEnumerable().AsAsyncQueryable());
                return result;
            }
        }

        private static Func<Expression, IAsyncEnumerable<TResult?>> CreateAsyncStreamProviderWithTestData<TData, TResult>(IEnumerable<TData> items, Action<Expression>? onExecuteAsyncStream)
        {
            return ExecuteAsyncStream;

            IAsyncEnumerable<TResult?> ExecuteAsyncStream(Expression expression)
            {
                onExecuteAsyncStream?.Invoke(expression);

                var result = expression.ExecuteAsyncStream<TResult>(_ => items.ToAsyncEnumerable().AsAsyncQueryable());
                return result;
            }
        }

        private static Func<Expression, ValueTask<TResult?>> CreateAsyncDataProviderWithTestData<TData, TResult>(IEnumerable<TData> items, Action<Expression>? onExecuteAsyncQuery)
        {
            return ExecuteAsyncQuery;

            async ValueTask<TResult?> ExecuteAsyncQuery(Expression expression)
            {
                onExecuteAsyncQuery?.Invoke(expression);

                var result = await expression.ExecuteAsync<TResult>(_ => items.ToAsyncEnumerable().AsAsyncQueryable());
                return result;
            }
        }
    }
}