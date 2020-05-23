// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq;

    internal sealed class CastingExpressionExecutor<TResult> : ExpressionExecutor<TResult>
    {
        public CastingExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override TResult ConvertResult(object? queryResult) => (TResult)queryResult!;
    }
}
