// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using Aqua.Dynamic;
using Remote.Linq.Expressions;

public class AsyncDefaultExpressionExecutionContext(AsyncExpressionExecutor<IQueryable, DynamicObject?> parent, Expression expression)
    : AsyncExpressionExecutionContext<DynamicObject?>(parent, expression);