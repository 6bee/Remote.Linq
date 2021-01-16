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
        /// <returns>The mapped result of the query execution.</returns>
        public static IAsyncEnumerable<DynamicObject?> ExecuteAsyncStream(this Expression expression, Func<Type, IAsyncQueryable> asyncQueryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new DefaultReactiveAsyncStreamExpressionExecutor(asyncQueryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).ExecuteAsyncStream(expression);

        // TODO: add doc
        public static ValueTask<DynamicObject?> ExecuteAsync(this Expression expression, Func<Type, IAsyncQueryable> asyncQueryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new DefaultReactiveAsyncExpressionExecutor(asyncQueryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).ExecuteAsync(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncQueryableProvider">Delegate to provide <see cref="IAsyncQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static IAsyncEnumerable<TResult?> ExecuteAsyncStream<TResult>(this Expression expression, Func<Type, IAsyncQueryable> asyncQueryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            where TResult : class
            => new CastingReactiveAsyncStreamExpressionExecutor<TResult>(asyncQueryableProvider, typeResolver, canBeEvaluatedLocally).ExecuteAsyncStream(expression);

        // TODO: add doc
        public static ValueTask<TResult?> ExecuteAsync<TResult>(this Expression expression, Func<Type, IAsyncQueryable> asyncQueryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            where TResult : class
            => new CastingReactiveAsyncExpressionExecutor<TResult>(asyncQueryableProvider, typeResolver, canBeEvaluatedLocally).ExecuteAsync(expression);
    }
}