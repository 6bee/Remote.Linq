// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

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
            => _parent = parent.CheckNotNull();

        /// <summary>
        /// Composes and executes the query asynchronously based on the <see cref="RemoteLinq.Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same EF context instance are not supported. Use <see langword="await"/> to ensure
        /// that any asynchronous operations have completed before calling another method on the same context.
        /// </remarks>
        /// <param name="expression">The <see cref="RemoteLinq.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="ValueTask{TDataTranferObject}"/> that represents the mapped result of the query execution.
        /// </returns>
        protected async ValueTask<TDataTranferObject> ExecuteAsync(RemoteLinq.Expression expression, CancellationToken cancellation)
        {
            var ctx = Context;

            var preparedRemoteExpression = Prepare(expression);
            ctx.RemoteExpression = preparedRemoteExpression;

            var linqExpression = Transform(preparedRemoteExpression);

            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellation);
            ctx.SystemExpression = preparedLinqExpression;

            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellation).ConfigureAwait(false);

            var processedResult = ProcessResult(queryResult);

            var dynamicObjects = ConvertResult(processedResult);

            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        protected virtual SystemLinq.Expression PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation)
           => _parent.PrepareAsyncQuery(expression, cancellation);

        protected virtual ValueTask<object?> ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation)
            => _parent.ExecuteAsync(expression, cancellation);

        SystemLinq.Expression IAsyncExpressionExecutionDecorator<TDataTranferObject>.PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation)
            => PrepareAsyncQuery(expression, cancellation);

        ValueTask<object?> IAsyncExpressionExecutionDecorator<TDataTranferObject>.ExecuteAsync(SystemLinq.Expression expression, CancellationToken cancellation)
            => ExecuteAsync(expression, cancellation);
    }
}
