// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.TypeSystem;
    using System;

    internal sealed class CastingExpressionExecutor<TQueryable, TResult> : ExpressionExecutor<TQueryable, TResult>
    {
        public CastingExpressionExecutor(Func<Type, TQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override TResult ConvertResult(object? queryResult)
            => (TResult)queryResult!;
    }
}
