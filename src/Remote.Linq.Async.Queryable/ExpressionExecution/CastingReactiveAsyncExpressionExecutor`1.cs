// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution;

public sealed class CastingReactiveAsyncExpressionExecutor<TResult>(Func<Type, IAsyncQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
    : InteractiveAsyncExpressionExecutor<TResult?>(queryableProvider, context)
{
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