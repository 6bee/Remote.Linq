// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AsyncExpressionExecutor<TQueryable, TDataTranferObject> : ExpressionExecutor<TQueryable, TDataTranferObject>, IAsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncExpressionExecutor{TQueryable, TDataTranferObject}"/> class.
        /// </summary>
        protected AsyncExpressionExecutor(Func<Type, TQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Composes and executes the query asynchronously based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same EF context instance are not supported. Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on the same context.
        /// </remarks>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the mapped result of the query execution.
        /// </returns>
        public async ValueTask<TDataTranferObject> ExecuteAsync(Expression expression, CancellationToken cancellation = default)
        {
            var preparedRemoteExpression = Prepare(expression.CheckNotNull(nameof(expression)));
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellation);
            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellation).ConfigureAwait(false);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected virtual System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => Prepare(expression);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <remarks>
        /// <see cref="InvalidOperationException"/> get handled for failing
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource})"/> and
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>.
        /// Instead of throwing an exception, an array with the length of zero respectively two elements is returned.
        /// </remarks>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected virtual async ValueTask<object?> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
        {
            static Type GetResultType(Type type)
            {
                if (type.Implements(typeof(Task<>), out var taskResultType))
                {
                    return taskResultType[0];
                }

                if (type.Implements(typeof(ValueTask<>), out var valueTaskResultType))
                {
                    return valueTaskResultType[0];
                }

                return type;
            }

            expression.CheckNotNull(nameof(expression));
            try
            {
                return await ExecuteCoreAsync(expression, cancellation).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                if (string.Equals(ex.Message, "Sequence contains no elements", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(GetResultType(expression.Type), 0);
                }

                if (string.Equals(ex.Message, "Sequence contains no matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(GetResultType(expression.Type), 0);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(GetResultType(expression.Type), 2);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(GetResultType(expression.Type), 2);
                }

                throw;
            }
        }

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected abstract ValueTask<object?> ExecuteCoreAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation);

        System.Linq.Expressions.Expression IAsyncExpressionExecutionDecorator<TDataTranferObject>.PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => PrepareAsyncQuery(expression, cancellation);

        ValueTask<object?> IAsyncExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => ExecuteAsync(expression, cancellation);
    }
}
