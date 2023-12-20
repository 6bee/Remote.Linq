// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution;

using Aqua.TypeExtensions;
using Remote.Linq.EntityFramework.ExpressionVisitors;
using Remote.Linq.ExpressionExecution;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

public abstract class EntityFrameworkExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<IQueryable, TDataTranferObject>
{
    [SecuritySafeCritical]
    protected EntityFrameworkExpressionExecutor(DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
        : this(dbContext.GetQueryableSetProvider(), context)
    {
    }

    protected EntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
        : base(queryableProvider, context ?? new EntityFrameworkExpressionTranslatorContext())
    {
    }

    protected override RemoteLinq.Expression Prepare(RemoteLinq.Expression expression)
        => base.Prepare(expression)
        .MapIncludeQueryMethods(Context?.TypeResolver);

    protected override SystemLinq.Expression Prepare(SystemLinq.Expression expression)
        => base.Prepare(expression)
        .ReplaceParameterizedConstructorCallsForVariableQueryArguments();

    /// <summary>
    /// Prepares the query <see cref="SystemLinq.Expression"/> to be able to be executed.
    /// </summary>
    /// <param name="expression">The <see cref="SystemLinq.Expression"/> returned by the Transform method.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="SystemLinq.Expression"/> ready for execution.</returns>
    protected override SystemLinq.Expression PrepareAsyncQuery(SystemLinq.Expression expression, CancellationToken cancellation)
        => Prepare(expression).ScalarQueryToAsyncExpression(cancellation);

    /// <summary>
    /// Executes the <see cref="SystemLinq.Expression"/> and returns the raw result.
    /// </summary>
    /// <param name="expression">The <see cref="SystemLinq.Expression"/> to be executed.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>Execution result of the <see cref="SystemLinq.Expression"/> specified.</returns>
    protected override async ValueTask<object?> ExecuteCoreAsync(SystemLinq.Expression expression, CancellationToken cancellation)
    {
        cancellation.ThrowIfCancellationRequested();

        var queryResult = expression.CheckNotNull().CompileAndInvokeExpression();

        if (queryResult is not null && queryResult.GetType().Implements(typeof(ValueTask<>), out var genericArguments))
        {
            var m = typeof(ValueTask<>).MakeGenericType(genericArguments).GetMethodEx(nameof(ValueTask<int>.AsTask));
            queryResult = m.Invoke(queryResult, null);
        }

        if (queryResult is Task task)
        {
            if (!expression.Type.Implements(typeof(Task<>), out var resultType))
            {
                resultType = task
                    .GetType()
                    .GetGenericArguments()
                    .ToArray();
            }

            if (resultType.Length is not 1)
            {
                throw new RemoteLinqException($"Failed to retrieve the result type for async query result {task.GetType()}");
            }

            queryResult = await GetTaskResultAsync(task, resultType[0]).ConfigureAwait(false);
        }

        if (queryResult is null)
        {
            return null;
        }

        cancellation.ThrowIfCancellationRequested();

        if (queryResult is IQueryable queryable)
        {
            // force query execution
            task = Helper.ToListAsync(queryable, cancellation);
            queryResult = await GetTaskResultAsync(task, typeof(List<>).MakeGenericType(queryable.ElementType)).ConfigureAwait(false);
        }

        return queryResult;
    }

    private static Task<TResult> AsTask<TResult>(ValueTask<TResult> valueTask) => valueTask.AsTask();

    private static async Task<object?> GetTaskResultAsync(Task task, Type resultType)
    {
        await task.ConfigureAwait(false);
        return TaskResultProperty(resultType).GetValue(task);
    }

    private static System.Reflection.PropertyInfo TaskResultProperty(Type resultType)
        => typeof(Task<>).MakeGenericType(resultType)
        .GetProperty(nameof(Task<object?>.Result))!;
}