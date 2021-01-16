// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class InteractiveAsyncExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<IAsyncQueryable, TDataTranferObject>
    {
        protected InteractiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected async override ValueTask<object?> ExecuteCoreAsync(Expression expression, CancellationToken cancellation)
        {
            var queryResult = expression.CheckNotNull(nameof(expression)).CompileAndInvokeExpression(); // TODO: execute expression async
            if (queryResult is null)
            {
                return null;
            }

            var queryResultType = queryResult.GetType();
            if (queryResultType.Implements(typeof(ValueTask<>), out var valueTaskResultType))
            {
                var m = typeof(ValueTask<>).MakeGenericType(valueTaskResultType).GetMethod(nameof(ValueTask<int>.AsTask));
                queryResult = m.Invoke(queryResult, null);
                queryResultType = queryResult.GetType();
            }

            if (queryResult is Task<object?> task)
            {
                return await task.ConfigureAwait(false);
            }

            if (queryResultType.Implements(typeof(IAsyncEnumerable<>), out var types1))
            {
                return queryResult;
            }

            if (queryResultType.Implements(typeof(Task<>), out var taskResultType))
            {
                return await Helper.MapTaskResultAsync(queryResult, taskResultType[0]).ConfigureAwait(false);
            }

            throw new RemoteLinqException($"Async query execution retured unexpected result: '{queryResultType.FullName}'");
        }
    }
}
