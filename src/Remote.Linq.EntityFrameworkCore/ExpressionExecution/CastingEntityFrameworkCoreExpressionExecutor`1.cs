// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Security;

    internal sealed class CastingEntityFrameworkCoreExpressionExecutor<TResult> : EntityFrameworkCoreExpressionExecutor<TResult>
    {
        [SecuritySafeCritical]
        public CastingEntityFrameworkCoreExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(dbContext, typeResolver, canBeEvaluatedLocally)
        {
        }

        public CastingEntityFrameworkCoreExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override TResult ConvertResult(object? queryResult) => (TResult)queryResult!;
    }
}
