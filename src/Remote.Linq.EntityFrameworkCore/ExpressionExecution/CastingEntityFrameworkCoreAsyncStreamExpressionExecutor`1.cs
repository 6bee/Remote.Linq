// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    internal sealed class CastingEntityFrameworkCoreAsyncStreamExpressionExecutor<TResult> : EntityFrameworkCoreAsyncStreamExpressionExecutor<TResult?>
        where TResult : class
    {
        [SecuritySafeCritical]
        public CastingEntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
            : base(dbContext, context)
        {
        }

        public CastingEntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
            : base(queryableProvider, context)
        {
        }

        protected override async IAsyncEnumerable<TResult?> ConvertResult(IAsyncEnumerable<object?> queryResult)
        {
            await foreach (var item in queryResult)
            {
                yield return item as TResult;
            }
        }
    }
}