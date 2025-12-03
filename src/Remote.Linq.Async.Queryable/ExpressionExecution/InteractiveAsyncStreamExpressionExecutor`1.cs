// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution;

using Aqua.TypeExtensions;
using Remote.Linq.ExpressionExecution;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

public abstract class InteractiveAsyncStreamExpressionExecutor<TDataTranferObject> : AsyncStreamExpressionExecutor<IAsyncQueryable, TDataTranferObject>
{
    protected InteractiveAsyncStreamExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context)
    {
    }

    protected override async IAsyncEnumerable<object?> ExecuteAsyncStream(Expression expression, [EnumeratorCancellation] CancellationToken cancellation)
    {
        var asyncEnumerable = ExecuteAsyncStreamCore(expression);
        await foreach (var item in asyncEnumerable.WithCancellation(cancellation).ConfigureAwait(false))
        {
            yield return item;
        }
    }

    private IAsyncEnumerable<object?> ExecuteAsyncStreamCore(Expression expression)
    {
        var queryResult = expression.CheckNotNull().CompileAndInvokeExpression();
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
                var m = typeof(ValueTask<>).MakeGenericType(genericArguments).GetMethodEx(nameof(ValueTask<>.AsTask));
                queryResult = m.Invoke(queryResult, null)!;
                queryResultType = queryResult!.GetType();
            }

            if (queryResultType.Implements(typeof(Task<>), out genericArguments))
            {
                return Helper.TaskResultToSingleElementStream(queryResult, genericArguments[0]);
            }
        }

        throw new RemoteLinqException($"Async stream execution retured unexpected result: '{queryResult?.GetType().FullName ?? "null"}'");
    }
}