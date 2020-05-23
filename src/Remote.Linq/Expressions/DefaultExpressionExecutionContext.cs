// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System.Collections.Generic;

    public class DefaultExpressionExecutionContext : ExpressionExecutionContext<IEnumerable<DynamicObject?>?>
    {
        public DefaultExpressionExecutionContext(ExpressionExecutor<IEnumerable<DynamicObject?>?> parent, Expression expression)
            : base(parent, expression)
        {
        }
    }
}
