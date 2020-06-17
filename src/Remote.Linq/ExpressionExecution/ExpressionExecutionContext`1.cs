// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;

    public class ExpressionExecutionContext<TDataTranferObject> : ExpressionExecutionDecoratorBase<TDataTranferObject>
    {
        private readonly Expression _expression;

        public ExpressionExecutionContext(ExpressionExecutionContext<TDataTranferObject> parent)
            : this(parent.CheckNotNull(nameof(parent)), parent._expression)
        {
        }

        public ExpressionExecutionContext(ExpressionExecutor<TDataTranferObject> parent, Expression expression)
            : this((IExpressionExecutionDecorator<TDataTranferObject>)parent, expression)
        {
        }

        internal ExpressionExecutionContext(IExpressionExecutionDecorator<TDataTranferObject> parent, Expression expression)
            : base(parent)
        {
            _expression = expression.CheckNotNull(nameof(expression));
        }

        public TDataTranferObject Execute() => Execute(_expression);
    }
}
