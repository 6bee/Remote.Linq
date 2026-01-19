// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

internal sealed class CastingExpressionExecutor<TQueryable, TResult>(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context)
    : ExpressionExecutor<TQueryable, TResult>(queryableProvider, context)
{
    protected override TResult ConvertResult(object? queryResult)
        => (TResult)queryResult!;
}