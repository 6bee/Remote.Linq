// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AsyncExpressionExecutionDecoratorBase<TDataTranferObject> : ExpressionExecutionDecoratorBase<TDataTranferObject>, IAsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly IAsyncExpressionExecutionDecorator<TDataTranferObject> _parent;

        protected AsyncExpressionExecutionDecoratorBase(AsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : this((IAsyncExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        protected AsyncExpressionExecutionDecoratorBase(AsyncExpressionExecutor<TDataTranferObject> parent)
            : this((IAsyncExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        internal AsyncExpressionExecutionDecoratorBase(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Composes and executes the query asynchronously based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same EF context instance are not supported. Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on the same context.
        /// </remarks>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the mapped result of the query execution.
        /// </returns>
        protected async Task<TDataTranferObject> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellationToken);
            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellationToken).ConfigureAwait(false);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        protected virtual System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
           => _parent.PrepareAsyncQuery(expression, cancellationToken);

        protected virtual Task<object?> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            => _parent.ExecuteAsync(expression, cancellationToken);

        System.Linq.Expressions.Expression IAsyncExpressionExecutionDecorator<TDataTranferObject>.PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            => PrepareAsyncQuery(expression, cancellationToken);

        Task<object?> IAsyncExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            => ExecuteAsync(expression, cancellationToken);
    }
}
