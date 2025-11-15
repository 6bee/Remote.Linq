// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;

public class QueryService
{
    public ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
    {
        using NHContext nhContext = new NHContext();
        return new ValueTask<DynamicObject>(queryExpression.Execute(nhContext.GetQueryable));
    }
}