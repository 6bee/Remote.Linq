// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Remote.Linq.Expressions;
    using System.Diagnostics.CodeAnalysis;

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

        [SuppressMessage("Major Code Smell", "S3442:\"abstract\" classes should not have \"public\" constructors", Justification = "Argument type has internal visibility only")]
        internal ExpressionExecutionDecorator(IExpressionExecutionDecorator<TDataTranferObject> parent)
            : base(parent)
        {
        }

        public new TDataTranferObject Execute(Expression expression) => base.Execute(expression);
    }
}
