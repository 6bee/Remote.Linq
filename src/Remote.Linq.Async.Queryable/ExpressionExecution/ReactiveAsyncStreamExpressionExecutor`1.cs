// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class ReactiveAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<IAsyncQueryable, TDataTranferObject>
        where TDataTranferObject : class
    {
        protected ReactiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override IAsyncEnumerable<object?> ExecuteAsyncStream(Expression expression)
        {
            var queryResult = expression.CheckNotNull(nameof(expression)).CompileAndInvokeExpression();
            if (queryResult is IAsyncEnumerable<object?> asyncEnumerable)
            {
                return asyncEnumerable;
            }

            if (queryResult is IEnumerable enumerable)
            {
                return enumerable.Cast<object?>().ToAsyncEnumerable();
            }

            throw new RemoteLinqException($"Async stream execution retured unexpected result: '{queryResult?.GetType().FullName ?? "null"}'");
        }
    }
}
