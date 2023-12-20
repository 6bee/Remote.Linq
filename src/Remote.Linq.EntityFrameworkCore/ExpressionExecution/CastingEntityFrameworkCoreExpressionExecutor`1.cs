// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security;

internal sealed class CastingEntityFrameworkCoreExpressionExecutor<TResult> : EntityFrameworkCoreExpressionExecutor<TResult>
{
    [SecuritySafeCritical]
    public CastingEntityFrameworkCoreExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
        : base(dbContext, context)
    {
    }

    public CastingEntityFrameworkCoreExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context)
    {
    }

    protected override TResult ConvertResult(object? queryResult) => (TResult)queryResult!;
}