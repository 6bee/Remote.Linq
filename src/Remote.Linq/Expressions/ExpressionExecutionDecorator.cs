// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System.Collections.Generic;

    public abstract class ExpressionExecutionDecorator : ExpressionExecutionDecoratorBase
    {
        protected ExpressionExecutionDecorator(ExpressionExecutionDecorator parent)
            : base(parent)
        {
        }

        protected ExpressionExecutionDecorator(ExpressionExecutor parent)
            : base(parent)
        {
        }

        internal ExpressionExecutionDecorator(IExpressionExecutionDecorator parent)
            : base(parent)
        {
        }

        public new IEnumerable<DynamicObject> Execute(Expression expression)
            => base.Execute(expression);
    }
}
