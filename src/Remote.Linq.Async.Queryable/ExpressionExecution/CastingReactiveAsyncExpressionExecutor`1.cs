// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution;

using System;
using System.Linq;

public sealed class CastingReactiveAsyncExpressionExecutor<TResult> : InteractiveAsyncExpressionExecutor<TResult?>
{
    public CastingReactiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context)
    {
    }

    protected override TResult? ConvertResult(object? queryResult)
    {
        if (queryResult is null)
        {
            return default;
        }

        if (queryResult is TResult result)
        {
            return result;
        }

        throw new NotImplementedException(nameof(ConvertResult));
    }
}