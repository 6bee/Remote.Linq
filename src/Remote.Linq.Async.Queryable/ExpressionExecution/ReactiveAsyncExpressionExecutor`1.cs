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
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class ReactiveAsyncExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<IAsyncQueryable, TDataTranferObject>
    {
        protected ReactiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected async override Task<object?> ExecuteCoreAsync(Expression expression, CancellationToken cancellation)
        {
            var queryResult = expression.CheckNotNull(nameof(expression)).CompileAndInvokeExpression(); // TODO: do async expression?
            if (queryResult is null)
            {
                return null;
            }

            if (queryResult.GetType().Implements(typeof(ValueTask<>), out var valueTaskResultType))
            {
                queryResult = CallAsTask(valueTaskResultType[0], queryResult);
            }

            if (queryResult is Task<object?> task)
            {
                return await task.ConfigureAwait(false);
            }

            if (queryResult.GetType().Implements(typeof(IAsyncEnumerable<>), out var types1))
            {
                return queryResult;
            }

            if (queryResult.GetType().Implements(typeof(Task<>), out var taskResultType))
            {
                return CallMapTaskAsync(taskResultType[0], queryResult);
            }

            throw new RemoteLinqException($"Async stream execution retured unexpected result: '{queryResult?.GetType().FullName ?? "null"}'");
        }

        private static readonly System.Reflection.MethodInfo _asTaskMethodInfo =
            typeof(ReactiveAsyncExpressionExecutor<TDataTranferObject>).GetMethod(nameof(AsTask), BindingFlags.NonPublic | BindingFlags.Static);

        private static object CallAsTask(Type resultType, object valueTask)
        {
            var method = _asTaskMethodInfo.MakeGenericMethod(resultType);
            var result = method.Invoke(null, new object[] { valueTask });
            return result;
        }

        private static Task<T> AsTask<T>(ValueTask<T> valueTask)
            => valueTask.AsTask();

        private static readonly System.Reflection.MethodInfo _mapTaskAsyncMethodInfo =
            typeof(ReactiveAsyncExpressionExecutor<TDataTranferObject>).GetMethod(nameof(MapTaskAsync), BindingFlags.NonPublic | BindingFlags.Static);

        private static object CallMapTaskAsync(Type resultType, object task)
        {
            var method = _mapTaskAsyncMethodInfo.MakeGenericMethod(resultType);
            var result = method.Invoke(null, new object[] { task });
            return result;
        }

        private static async Task<object?> MapTaskAsync<T>(Task<T> task)
            => await task.ConfigureAwait(false); // TODO: how-to relection and async?!
    }
}
