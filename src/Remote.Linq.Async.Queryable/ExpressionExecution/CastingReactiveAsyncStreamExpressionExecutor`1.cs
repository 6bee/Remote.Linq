// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution;

using System;
using System.Collections.Generic;
using System.Linq;

internal sealed class CastingReactiveAsyncStreamExpressionExecutor<TResult> : InteractiveAsyncStreamExpressionExecutor<TResult>
{
    public CastingReactiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context)
    {
    }

    protected override async IAsyncEnumerable<TResult> ConvertResult(IAsyncEnumerable<object?> queryResult)
    {
        if (queryResult is not null)
        {
            await foreach (var item in queryResult)
            {
                yield return (TResult)item!;
            }
        }
    }
}