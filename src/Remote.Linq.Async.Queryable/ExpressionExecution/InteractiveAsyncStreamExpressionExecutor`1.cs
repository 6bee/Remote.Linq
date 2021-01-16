// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public abstract class InteractiveAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<IAsyncQueryable, TDataTranferObject>
        where TDataTranferObject : class
    {
        protected InteractiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<Expression, bool>? canBeEvaluatedLocally = null)
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

            if (queryResult is not null)
            {
                var queryResultType = queryResult.GetType();
                if (queryResultType.Implements(typeof(IAsyncEnumerable<>), out var genericArguments))
                {
                    return Helper.MapAsyncEnumerable(queryResult, genericArguments[0]);
                }

                if (queryResultType.Implements(typeof(ValueTask<>), out genericArguments))
                {
                    var m = typeof(ValueTask<>).MakeGenericType(genericArguments).GetMethod(nameof(ValueTask<int>.AsTask));
                    queryResult = m.Invoke(queryResult, null);
                    queryResultType = queryResult.GetType();
                }

                if (queryResultType.Implements(typeof(Task<>), out genericArguments))
                {
                    return Helper.TaskResultToSingleElementStream(queryResult, genericArguments[0]);
                }
            }

            throw new RemoteLinqException($"Async stream execution retured unexpected result: '{queryResult?.GetType().FullName ?? "null"}'");
        }
    }
}
