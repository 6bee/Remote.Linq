// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;

    public class AsyncDefaultExpressionExecutionContext : AsyncExpressionExecutionContext<IEnumerable<DynamicObject?>?>
    {
        public AsyncDefaultExpressionExecutionContext(AsyncExpressionExecutor<IEnumerable<DynamicObject?>?> parent, Expression expression)
            : base(parent, expression)
        {
        }
    }
}
