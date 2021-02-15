// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncExpressionExecutionContext<TDataTranferObject> : AsyncExpressionExecutionDecoratorBase<TDataTranferObject>
    {
        private readonly Expression _expression;

        public AsyncExpressionExecutionContext(AsyncExpressionExecutionContext<TDataTranferObject> parent)
            : this(parent.CheckNotNull(nameof(parent)), parent._expression)
        {
        }

        public AsyncExpressionExecutionContext(AsyncExpressionExecutor<IQueryable, TDataTranferObject> parent, Expression expression)
            : this((IAsyncExpressionExecutionDecorator<TDataTranferObject>)parent, expression)
        {
        }

        internal AsyncExpressionExecutionContext(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent, Expression expression)
            : base(parent)
            => _expression = expression.CheckNotNull(nameof(expression));

        public TDataTranferObject Execute()
            => Execute(_expression);

        public ValueTask<TDataTranferObject> ExecuteAsync(CancellationToken cancellation)
            => ExecuteAsync(_expression, cancellation);
    }
}
