// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AsyncExpressionExecutionDecorator<TDataTranferObject> : AsyncExpressionExecutionDecoratorBase<TDataTranferObject>
    {
        protected AsyncExpressionExecutionDecorator(AsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        protected AsyncExpressionExecutionDecorator(AsyncExpressionExecutor<TDataTranferObject> parent)
            : base(parent)
        {
        }

        internal AsyncExpressionExecutionDecorator(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        public new TDataTranferObject Execute(Expression expression) => base.Execute(expression);

        public new Task<TDataTranferObject> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default) => base.ExecuteAsync(expression, cancellationToken);
    }
}
