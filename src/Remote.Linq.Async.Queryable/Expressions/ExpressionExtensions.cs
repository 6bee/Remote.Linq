// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Async.Queryable.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncQueryableProvider">Delegate to provide <see cref="IAsyncQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>An asynchronous stream of query results.</returns>
        public static IAsyncEnumerable<DynamicObject?> ExecuteAsyncStream(
            this Expression expression,
            Func<Type, IAsyncQueryable> asyncQueryableProvider,
            ITypeResolver? typeResolver = null,
            IDynamicObjectMapper? mapper = null,
            Func<Type, bool>? setTypeInformation = null,
            Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null,
            CancellationToken cancellation = default)
            => new DefaultReactiveAsyncStreamExpressionExecutor(asyncQueryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).ExecuteAsyncStream(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncQueryableProvider">Delegate to provide <see cref="IAsyncQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>The asynchronous query result.</returns>
        public static ValueTask<DynamicObject?> ExecuteAsync(
            this Expression expression,
            Func<Type, IAsyncQueryable> asyncQueryableProvider,
            ITypeResolver? typeResolver = null,
            IDynamicObjectMapper? mapper = null,
            Func<Type, bool>? setTypeInformation = null,
            Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null,
            CancellationToken cancellation = default)
            => new DefaultReactiveAsyncExpressionExecutor(asyncQueryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).ExecuteAsync(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> as an asynchronous stream.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncQueryableProvider">Delegate to provide <see cref="IAsyncQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>An asynchronous stream of query results.</returns>
        public static IAsyncEnumerable<TResult?> ExecuteAsyncStream<TResult>(
            this Expression expression,
            Func<Type, IAsyncQueryable> asyncQueryableProvider,
            ITypeResolver? typeResolver = null,
            Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null,
            CancellationToken cancellation = default)
            => new CastingReactiveAsyncStreamExpressionExecutor<TResult>(asyncQueryableProvider, typeResolver, canBeEvaluatedLocally).ExecuteAsyncStream(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> as an asynchronous operation.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncQueryableProvider">Delegate to provide <see cref="IAsyncQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the async operation to complete.</param>
        /// <returns>The asynchronous query result.</returns>
        public static ValueTask<TResult?> ExecuteAsync<TResult>(
            this Expression expression,
            Func<Type, IAsyncQueryable> asyncQueryableProvider,
            ITypeResolver? typeResolver = null,
            Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null,
            CancellationToken cancellation = default)
            => new CastingReactiveAsyncExpressionExecutor<TResult>(asyncQueryableProvider, typeResolver, canBeEvaluatedLocally).ExecuteAsync(expression, cancellation);
    }
}