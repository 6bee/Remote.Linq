// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution;

using System.Data.Entity;
using System.Security;

public sealed class CastingEntityFrameworkExpressionExecutor<TResult> : EntityFrameworkExpressionExecutor<TResult>
{
    [SecuritySafeCritical]
    public CastingEntityFrameworkExpressionExecutor(DbContext dbContext, IExpressionTranslatorContext? context = null)
        : base(dbContext, context)
    {
    }

    public CastingEntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionTranslatorContext? context = null)
        : base(queryableProvider, context)
    {
    }

    protected override TResult ConvertResult(object? queryResult) => (TResult)queryResult!;
}