// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public abstract class AsyncExpressionExecutor<TQueryable, TDataTranferObject> : ExpressionExecutor<TQueryable, TDataTranferObject>, IAsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncExpressionExecutor{TQueryable, TDataTranferObject}"/> class.
        /// </summary>
        protected AsyncExpressionExecutor(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
            : base(queryableProvider, context)
        {
        }

        /// <summary>
        /// Composes and executes the query asynchronously based on the <see cref="RemoteLinq.Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same EF context instance are not supported. Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on the same context.
        /// </remarks>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the mapped result of the query execution.
        /// </returns>
        public async ValueTask<TDataTranferObject> ExecuteAsync(RemoteLinq.Expression expression, CancellationToken cancellation = default)
        {
            expression.AssertNotNull(nameof(expression));

            var ctx = Context;

            var preparedRemoteExpression = Prepare(expression);
            ctx.RemoteExpression = preparedRemoteExpression;

            var linqExpression = Transform(preparedRemoteExpression);

            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellation);
            ctx.SystemExpression = preparedLinqExpression;

            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellation).ConfigureAwait(false);

            var processedResult = ProcessResult(queryResult);

            var dataTransferObjects = ConvertResult(processedResult);

            var processedDataTransferObjects = ProcessResult(dataTransferObjects);
            return processedDataTransferObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="SystemLinq.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> returned by the Transform method.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="SystemLinq.Expression"/> ready for execution.</returns>
        protected virtual SystemLinq.Expression PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation)
            => Prepare(expression);

        /// <summary>
        /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw result.
        /// </summary>
        /// <remarks>
        /// <see cref="InvalidOperationException"/> get handled for failing
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource})"/> and
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource}, SystemLinq.Expression{Func{TSource, bool}})"/>.
        /// Instead of throwing an exception, an array with the length of zero respectively two elements is returned.
        /// </remarks>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
        protected virtual async ValueTask<object?> ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation)
        {
            try
            {
                return await ExecuteCoreAsync(expression, cancellation).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                if (string.Equals(ex.Message, "Sequence contains no elements", StringComparison.Ordinal))
                {
                    return CreateArray(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains no matching element", StringComparison.Ordinal))
                {
                    return CreateArray(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one element", StringComparison.Ordinal))
                {
                    return CreateArray(expression.Type, 2);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one matching element", StringComparison.Ordinal))
                {
                    return CreateArray(expression.Type, 2);
                }

                throw;
            }

            static Array CreateArray(Type type, int size) => Array.CreateInstance(GetResultType(type), size);

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

                if (!type.IsNullableType())
                {
                    return typeof(Nullable<>).MakeGenericType(type);
                }

                return type;
            }
        }

        /// <summary>
        /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="SystemLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
        protected abstract ValueTask<object?> ExecuteCoreAsync(SystemLinq.Expression expression, CancellationToken cancellation);

        SystemLinq.Expression IAsyncExpressionExecutionDecorator<TDataTranferObject>.PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation)
            => PrepareAsyncQuery(expression, cancellation);

        ValueTask<object?> IAsyncExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation)
            => ExecuteAsync(expression, cancellation);
    }
}