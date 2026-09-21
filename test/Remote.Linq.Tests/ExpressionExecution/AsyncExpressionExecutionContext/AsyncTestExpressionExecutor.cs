// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.AsyncExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;

public class AsyncTestExpressionExecutor : AsyncExpressionExecutor<IQueryable, DynamicObject>
{
    public AsyncTestExpressionExecutor()
        : base(null, null)
    {
    }

    protected override DynamicObject ConvertResult(object queryResult)
        => throw new NotSupportedException("Test scenario does actually invoke expression executor");

    protected override ValueTask<object> ExecuteCoreAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
        => throw new NotSupportedException("Test scenario does actually invoke expression executor");
}
