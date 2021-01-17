// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class CastingReactiveAsyncStreamExpressionExecutor<TResult> : InteractiveAsyncStreamExpressionExecutor<TResult>
    {
        public CastingReactiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver, Func<Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override async IAsyncEnumerable<TResult?> ConvertResult(IAsyncEnumerable<object?> queryResult)
        {
            await foreach (var item in queryResult)
            {
                yield return (TResult?)item;
            }
        }
    }
}
