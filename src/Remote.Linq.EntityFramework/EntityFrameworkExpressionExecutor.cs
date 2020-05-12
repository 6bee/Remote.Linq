// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public class EntityFrameworkExpressionExecutor : ExpressionExecutor
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

        private static readonly Func<Task, object?> GetTaskResult = t => TaskResultProperty(t.GetType().GetGenericArguments().Single()).GetValue(t);

        [SecuritySafeCritical]
        public EntityFrameworkExpressionExecutor(DbContext dbContext, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : this(GetQueryableSetProvider(dbContext), typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally)
        {
        }

        public EntityFrameworkExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            : base(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally)
        {
        }

        /// <summary>
        /// Composes and executes the query asynchronously based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <remarks>
        /// Multiple active operations on the same EF context instance are not supported. Use 'await' to ensure
        /// that any asynchronous operations have completed before calling another method on the same context.
        /// </remarks>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the mapped result of the query execution.
        /// </returns>
        public async Task<IEnumerable<DynamicObject?>?> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = PrepareAsyncQuery(linqExpression, cancellationToken);
            var queryResult = await ExecuteAsync(preparedLinqExpression, cancellationToken).ConfigureAwait(false);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected virtual System.Linq.Expressions.Expression PrepareAsyncQuery(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
            => Prepare(expression).ScalarQueryToAsyncExpression(cancellationToken);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <remarks>
        /// <see cref="InvalidOperationException"/> get handled for failing
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource})"/> and
        /// <see cref="Queryable.Single{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.First{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource})"/>,
        /// <see cref="Queryable.Last{TSource}(IQueryable{TSource}, System.Linq.Expressions.Expression{Func{TSource, bool}})"/>.
        /// Instead of throwing an exception, an array with the length of zero respectively two elements is returned.
        /// </remarks>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected virtual async Task<object?> ExecuteAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
        {
            try
            {
                return await ExecuteCoreAsync(expression, cancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                if (string.Equals(ex.Message, "Sequence contains no elements", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains no matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 0);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 2);
                }

                if (string.Equals(ex.Message, "Sequence contains more than one matching element", StringComparison.Ordinal))
                {
                    return Array.CreateInstance(expression.Type, 2);
                }

                throw;
            }
        }

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected async Task<object?> ExecuteCoreAsync(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken)
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

        // temporary implementation for compatibility with previous versions
        internal System.Linq.Expressions.Expression PrepareForExecution(Expression expression)
            => Prepare(Transform(Prepare(expression)));

        /// <summary>
        /// Returns the generic <see cref="DbSet{T}"/> for the type specified.
        /// </summary>
        /// <returns>Returns an instance of type <see cref="DbSet{T}"/>.</returns>
        [SecuritySafeCritical]
        private static Func<Type, IQueryable> GetQueryableSetProvider(DbContext dbContext) => new QueryableSetProvider(dbContext).GetQueryableSet;

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
    }
}
