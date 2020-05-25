// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class EntityFrameworkCoreExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<TDataTranferObject>
    {
        private static readonly System.Reflection.MethodInfo ToListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetTypeInfo()
            .GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ToListAsync))
            .Where(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        private static readonly Func<Type, System.Reflection.PropertyInfo> TaskResultProperty = (Type resultType) =>
            typeof(Task<>).MakeGenericType(resultType)
                .GetProperty(nameof(Task<object?>.Result));

        [SecuritySafeCritical]
        protected EntityFrameworkCoreExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(dbContext.GetQueryableSetProvider(), typeResolver, canBeEvaluatedLocally.And(ExpressionEvaluator.CanBeEvaluated))
        {
        }

        protected EntityFrameworkCoreExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected override System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            => Prepare(expression).ScalarQueryToAsyncExpression(cancellationToken);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected override async Task<object?> ExecuteCoreAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResult = expression.CompileAndInvokeExpression();
            if (queryResult is Task task)
            {
                queryResult = await GetTaskResultAsync(task).ConfigureAwait(false);
            }

            if (queryResult is null)
            {
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var queryableType = queryResult.GetType();
            if (queryableType.Implements(typeof(IQueryable<>)))
            {
                // force query execution
                var elementType = TypeHelper.GetElementType(queryableType);
                task = (Task)ToListAsync.MakeGenericMethod(elementType).Invoke(null, new[] { queryResult, cancellationToken });
                queryResult = await GetTaskResultAsync(task).ConfigureAwait(false);
            }

            return queryResult;
        }

        protected override Expression Prepare(Expression expression) => base.Prepare(expression).ReplaceIncludeMethodCall();

        private static async Task<object?> GetTaskResultAsync(Task task)
        {
            await task.ConfigureAwait(false);
            return GetTaskResult(task);
        }

        private static object? GetTaskResult(Task task) => TaskResultProperty(task.GetType().GetGenericArguments().Single()).GetValue(task);
    }
}
