// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq.EntityFrameworkCore.ExpressionExecution;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "CancellationToken as first optional parameter")]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Creates an <see cref="AsyncDefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
        [SecuritySafeCritical]
        public static AsyncDefaultExpressionExecutionContext EntityFrameworkCoreExecutor(
            this Expression expression,
            DbContext dbContext,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => new AsyncDefaultExpressionExecutionContext(new DefaultEntityFrameworkCoreExpressionExecutor(dbContext, context, setTypeInformation), expression);

        /// <summary>
        /// Creates an <see cref="AsyncDefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
        public static AsyncDefaultExpressionExecutionContext EntityFrameworkCoreExecutor(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => new AsyncDefaultExpressionExecutionContext(new DefaultEntityFrameworkCoreExpressionExecutor(queryableProvider, context, setTypeInformation), expression);

        /// <summary>
        /// Creates an <see cref="AsyncExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>A new instance <see cref="AsyncExpressionExecutionContext{TResult}" />.</returns>
        [SecuritySafeCritical]
        public static AsyncExpressionExecutionContext<TResult> EntityFrameworkCoreExecutor<TResult>(
            this Expression expression,
            DbContext dbContext,
            IExpressionFromRemoteLinqContext? context = null)
            => new AsyncExpressionExecutionContext<TResult>(new CastingEntityFrameworkCoreExpressionExecutor<TResult>(dbContext, context), expression);

        /// <summary>
        /// Creates an <see cref="AsyncExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>A new instance <see cref="AsyncExpressionExecutionContext{TResult}" />.</returns>
        public static AsyncExpressionExecutionContext<TResult> EntityFrameworkCoreExecutor<TResult>(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null)
            => new AsyncExpressionExecutionContext<TResult>(new CastingEntityFrameworkCoreExpressionExecutor<TResult>(queryableProvider, context), expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        [SecuritySafeCritical]
        public static DynamicObject? ExecuteWithEntityFrameworkCore(
            this Expression expression,
            DbContext dbContext,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => new DefaultEntityFrameworkCoreExpressionExecutor(dbContext, context, setTypeInformation).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static DynamicObject? ExecuteWithEntityFrameworkCore(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => new DefaultEntityFrameworkCoreExpressionExecutor(queryableProvider, context, setTypeInformation).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The result of the query execution.</returns>
        [SecuritySafeCritical]
        public static TResult ExecuteWithEntityFrameworkCore<TResult>(this Expression expression, DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
            => new CastingEntityFrameworkCoreExpressionExecutor<TResult>(dbContext, context).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The result of the query execution.</returns>
        public static TResult ExecuteWithEntityFrameworkCore<TResult>(this Expression expression, Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null)
            => new CastingEntityFrameworkCoreExpressionExecutor<TResult>(queryableProvider, context).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        [SecuritySafeCritical]
        public static ValueTask<DynamicObject?> ExecuteWithEntityFrameworkCoreAsync(this Expression expression, DbContext dbContext, CancellationToken cancellation = default, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            => new DefaultEntityFrameworkCoreExpressionExecutor(dbContext, context, setTypeInformation).ExecuteAsync(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static ValueTask<DynamicObject?> ExecuteWithEntityFrameworkCoreAsync(this Expression expression, Func<Type, IQueryable> queryableProvider, CancellationToken cancellation = default, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            => new DefaultEntityFrameworkCoreExpressionExecutor(queryableProvider, context, setTypeInformation).ExecuteAsync(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The result of the query execution.</returns>
        [SecuritySafeCritical]
        public static ValueTask<TResult> ExecuteWithEntityFrameworkCoreAsync<TResult>(this Expression expression, DbContext dbContext, CancellationToken cancellation = default, IExpressionFromRemoteLinqContext? context = null)
            => new CastingEntityFrameworkCoreExpressionExecutor<TResult>(dbContext, context).ExecuteAsync(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The result of the query execution.</returns>
        public static ValueTask<TResult> ExecuteWithEntityFrameworkCoreAsync<TResult>(this Expression expression, Func<Type, IQueryable> queryableProvider, CancellationToken cancellation = default, IExpressionFromRemoteLinqContext? context = null)
            => new CastingEntityFrameworkCoreExpressionExecutor<TResult>(queryableProvider, context).ExecuteAsync(expression, cancellation);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static IAsyncEnumerable<DynamicObject?> ExecuteAsyncStreamWithEntityFrameworkCore(this Expression expression, Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context = null, Func<Type, bool>? setTypeInformation = null)
            => new DefaultEntityFrameworkCoreAsyncStreamExpressionExecutor(queryableProvider, context, setTypeInformation).ExecuteAsyncStream(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The result of the query execution.</returns>
        [SecuritySafeCritical]
        public static IAsyncEnumerable<object?> ExecuteAsyncStreamWithEntityFrameworkCore(this Expression expression, DbContext dbContext, IExpressionFromRemoteLinqContext? context = null)
            => new CastingEntityFrameworkCoreAsyncStreamExpressionExecutor<object>(dbContext, context).ExecuteAsyncStream(expression);
    }
}