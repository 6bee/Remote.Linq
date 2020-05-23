// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution
{
    using Aqua.TypeSystem;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;

    internal sealed class CastingEntityFrameworkExpressionExecutor<TResult> : EntityFrameworkExpressionExecutor<TResult>
    {
        [SecuritySafeCritical]
        public CastingEntityFrameworkExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(dbContext, typeResolver, canBeEvaluatedLocally)
        {
        }

        public CastingEntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override TResult ConvertResult(object? queryResult) => (TResult)queryResult!;
    }
}
