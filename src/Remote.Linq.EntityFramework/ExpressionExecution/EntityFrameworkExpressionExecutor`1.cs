// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework.ExpressionExecution
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class EntityFrameworkExpressionExecutor<TDataTranferObject> : AsyncExpressionExecutor<TDataTranferObject>
    {
        private static readonly System.Reflection.MethodInfo DbContextSetMethod = typeof(DbContext)
            .GetMethods()
            .Single(x => x.Name == nameof(DbContext.Set) && x.IsGenericMethod && x.GetGenericArguments().Length == 1 && x.GetParameters().Length == 0);

        private static readonly System.Reflection.MethodInfo ToListAsync = typeof(QueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => string.Equals(m.Name, nameof(QueryableExtensions.ToListAsync), StringComparison.Ordinal))
            .Where(m => m.IsGenericMethodDefinition && m.GetParameters().Length == 2)
            .Single();

        private static readonly Func<Type, System.Reflection.PropertyInfo> TaskResultProperty = resultType =>
            typeof(Task<>).MakeGenericType(resultType)
                .GetProperty(nameof(Task<object?>.Result));

        [SecuritySafeCritical]
        protected EntityFrameworkExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(GetQueryableSetProvider(dbContext), typeResolver, canBeEvaluatedLocally)
        {
        }

        protected EntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
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

            var queryResult = CompileAndInvokeExpression(expression);
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
                task = ToListAsync.MakeGenericMethod(elementType).InvokeAndUnwrap<Task>(null, queryResult, cancellationToken);
                await task.ConfigureAwait(false);
                var result = TaskResultProperty(typeof(List<>).MakeGenericType(elementType)).GetValue(task);
                queryResult = result;
            }

            return queryResult;
        }

        protected override Expression Prepare(Expression expression)
            => base.Prepare(expression).ReplaceIncludeMethodCall();

        protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
            => base.Prepare(expression).ReplaceParameterizedConstructorCallsForVariableQueryArguments();

        /// <summary>
        /// Returns the generic <see cref="DbSet{T}"/> for the type specified.
        /// </summary>
        /// <returns>Returns an instance of type <see cref="DbSet{T}"/>.</returns>
        [SecuritySafeCritical]
        protected static Func<Type, IQueryable> GetQueryableSetProvider(DbContext dbContext) => new QueryableSetProvider(dbContext).GetQueryableSet;

        [SecuritySafeCritical]
        private sealed class QueryableSetProvider
        {
            private readonly DbContext _dbContext;

            [SecuritySafeCritical]
            public QueryableSetProvider(DbContext dbContext)
            {
                _dbContext = dbContext;
            }

            [SecuritySafeCritical]
            public IQueryable GetQueryableSet(Type type)
            {
                var method = DbContextSetMethod.MakeGenericMethod(type);
                var set = method.Invoke(_dbContext, null);
                return (IQueryable)set;
            }
        }

        private static async Task<object?> GetTaskResultAsync(Task task)
        {
            await task.ConfigureAwait(false);
            return GetTaskResult(task);
        }

        private static object? GetTaskResult(Task task) => TaskResultProperty(task.GetType().GetGenericArguments().Single()).GetValue(task);
    }
}
