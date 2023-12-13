// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class AsyncExpressionExecutionDecorator<TDataTranferObject> : AsyncExpressionExecutionDecoratorBase<TDataTranferObject>
    {
        protected AsyncExpressionExecutionDecorator(AsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        protected AsyncExpressionExecutionDecorator(AsyncExpressionExecutor<IQueryable, TDataTranferObject> parent)
            : base(parent)
        {
        }

        [SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "Argument type has internal visibility only")]
        internal AsyncExpressionExecutionDecorator(IAsyncExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        public new TDataTranferObject Execute(Expression expression)
            => base.Execute(expression);

        public new ValueTask<TDataTranferObject> ExecuteAsync(Expression expression, CancellationToken cancellation = default)
            => base.ExecuteAsync(expression, cancellation);
    }
}