// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System;
    using System.Collections.Generic;

    public class ExpressionExecutionContext : ExpressionExecutionDecoratorBase
    {
        private readonly Expression _expression;

        public ExpressionExecutionContext(ExpressionExecutionContext parent)
            : this(parent, parent._expression)
        {
        }

        public ExpressionExecutionContext(ExpressionExecutor parent, Expression expression)
            : this((IExpressionExecutionDecorator)parent, expression)
        {
        }

        internal ExpressionExecutionContext(IExpressionExecutionDecorator parent, Expression expression)
            : base(parent)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public IEnumerable<DynamicObject> Execute()
            => Execute(_expression);
    }
}
