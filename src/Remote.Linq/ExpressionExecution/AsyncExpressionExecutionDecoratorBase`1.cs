// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    public abstract class AsyncExpressionExecutionDecoratorBase<TDataTranferObject> : ExpressionExecutionDecoratorBase<TDataTranferObject>, IAsyncExpressionExecutionDecorator<TDataTranferObject>
    {
        private readonly IAsyncExpressionExecutionDecorator<TDataTranferObject> _parent;

        protected AsyncExpressionExecutionDecoratorBase(AsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : this((IAsyncExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        protected AsyncExpressionExecutionDecoratorBase(AsyncExpressionExecutor<IQueryable, TDataTranferObject> parent)
            : this((IAsyncExpressionExecutionDecorator<TDataTranferObject>)parent)
        {
        }

        [SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "Argument type has internal visibility only")]
        internal AsyncExpressionExecutionDecoratorBase(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
            _parent = parent.CheckNotNull(nameof(parent));
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
        /// A <see cref="ValueTask{TDataTranferObject}"/> that represents the mapped result of the query execution.
        /// </returns>
        protected async ValueTask<TDataTranferObject> ExecuteAsync(Expression expression, CancellationToken cancellation)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellation);
            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellation).ConfigureAwait(false);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        protected virtual System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
           => _parent.PrepareAsyncQuery(expression, cancellation);

        protected virtual ValueTask<object?> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => _parent.ExecuteAsync(expression, cancellation);

        System.Linq.Expressions.Expression IAsyncExpressionExecutionDecorator<TDataTranferObject>.PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => PrepareAsyncQuery(expression, cancellation);

        ValueTask<object?> IAsyncExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => ExecuteAsync(expression, cancellation);
    }
}
