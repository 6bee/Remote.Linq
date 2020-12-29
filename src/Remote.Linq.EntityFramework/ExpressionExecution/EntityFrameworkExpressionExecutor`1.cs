// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.EntityFramework.ExpressionVisitors;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class EntityFrameworkExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<IQueryable, TDataTranferObject>
    {
        [SecuritySafeCritical]
        protected EntityFrameworkExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, canBeEvaluatedLocally)
        {
        }

        protected EntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override Expression Prepare(Expression expression)
            => base.Prepare(expression).ReplaceIncludeMethodCall();

        protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
            => base.Prepare(expression).ReplaceParameterizedConstructorCallsForVariableQueryArguments();

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected override System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
            => Prepare(expression).ScalarQueryToAsyncExpression(cancellation);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected override async Task<object?> ExecuteCoreAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            var queryResult = expression.CheckNotNull(nameof(expression)).CompileAndInvokeExpression();
            if (queryResult is Task task)
            {
                if (!expression.Type.Implements(typeof(Task<>), out var resultType))
                {
                    resultType = task
                        .GetType()
                        .GetGenericArguments()
                        .Where(x => x != typeof(CancellationToken))
                        .ToArray();
                }

                if (resultType.Length != 1)
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

        private static async Task<object?> GetTaskResultAsync(Task task, Type resultType)
        {
            await task.ConfigureAwait(false);
            return TaskResultProperty(resultType).GetValue(task);
        }

        private static System.Reflection.PropertyInfo TaskResultProperty(Type resultType) =>
            typeof(Task<>).MakeGenericType(resultType)
                .GetProperty(nameof(Task<object?>.Result));
    }
}
