// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using SystemLinq = System.Linq.Expressions;

    internal sealed class CastingEntityFrameworkCoreAsyncStreamExpressionExecutor<TResult> : EntityFrameworkCoreAsyncStreamExpressionExecutor<TResult?>
        where TResult : class
    {
        [SecuritySafeCritical]
        public CastingEntityFrameworkCoreAsyncStreamExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : base(dbContext, typeResolver, canBeEvaluatedLocally)
        {
        }

        public CastingEntityFrameworkCoreAsyncStreamExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
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
