// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    public abstract class ExpressionExecutionDecorator<TDataTranferObject> : ExpressionExecutionDecoratorBase<TDataTranferObject>
    {
        protected ExpressionExecutionDecorator(ExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        protected ExpressionExecutionDecorator(ExpressionExecutor<TDataTranferObject> parent)
            : base(parent)
        {
        }

        internal ExpressionExecutionDecorator(IExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        public new TDataTranferObject Execute(Expression expression) => base.Execute(expression);
    }
}
