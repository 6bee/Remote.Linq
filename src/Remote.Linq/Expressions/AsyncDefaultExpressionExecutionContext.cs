// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using System.Collections.Generic;

    public class AsyncDefaultExpressionExecutionContext : AsyncExpressionExecutionContext<IEnumerable<DynamicObject?>?>
    {
        public AsyncDefaultExpressionExecutionContext(AsyncExpressionExecutor<IEnumerable<DynamicObject?>?> parent, Expression expression)
            : base(parent, expression)
        {
        }
    }
}
