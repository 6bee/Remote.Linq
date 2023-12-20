// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution;

using System;

internal sealed class CastingExpressionExecutor<TQueryable, TResult> : ExpressionExecutor<TQueryable, TResult>
{
    public CastingExpressionExecutor(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context)
        : base(queryableProvider, context)
    {
    }

    protected override TResult ConvertResult(object? queryResult)
        => (TResult)queryResult!;
}