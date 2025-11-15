// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework;

using Aqua.Dynamic;
using Aqua.TypeSystem;
using Remote.Linq.EntityFramework.ExpressionExecution;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Security;

[EditorBrowsable(EditorBrowsableState.Never)]
[SuppressMessage("Design", "CA1068:CancellationToken parameters must come last", Justification = "CancellationToken as first optional parameter")]
public static class ExpressionExtensions
{
    /// <summary>
    /// Creates an <see cref="AsyncDefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
    [SecuritySafeCritical]
    public static AsyncDefaultExpressionExecutionContext EntityFrameworkExecutor(
        this Expression expression,
        DbContext dbContext,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new AsyncDefaultExpressionExecutionContext(new DefaultEntityFrameworkExpressionExecutor(dbContext, context, setTypeInformation), expression);

    /// <summary>
    /// Creates an <see cref="AsyncDefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
    public static AsyncDefaultExpressionExecutionContext EntityFrameworkExecutor(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new AsyncDefaultExpressionExecutionContext(new DefaultEntityFrameworkExpressionExecutor(queryableProvider, context, setTypeInformation), expression);

    /// <summary>
    /// Creates an <see cref="AsyncExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>A new instance <see cref="AsyncExpressionExecutionContext{TResult}" />.</returns>
    [SecuritySafeCritical]
    public static AsyncExpressionExecutionContext<TResult> EntityFrameworkExecutor<TResult>(
        this Expression expression,
        DbContext dbContext,
        IExpressionTranslatorContext? context = null)
        => new AsyncExpressionExecutionContext<TResult>(new CastingEntityFrameworkExpressionExecutor<TResult>(dbContext, context), expression);

    /// <summary>
    /// Creates an <see cref="AsyncExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>A new instance <see cref="AsyncExpressionExecutionContext{TResult}" />.</returns>
    public static AsyncExpressionExecutionContext<TResult> EntityFrameworkExecutor<TResult>(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        IExpressionTranslatorContext? context = null)
        => new AsyncExpressionExecutionContext<TResult>(new CastingEntityFrameworkExpressionExecutor<TResult>(queryableProvider, context), expression);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>The mapped result of the query execution.</returns>
    [SecuritySafeCritical]
    public static DynamicObject? ExecuteWithEntityFramework(
        this Expression expression,
        DbContext dbContext,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new DefaultEntityFrameworkExpressionExecutor(dbContext, context, setTypeInformation).Execute(expression);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>The mapped result of the query execution.</returns>
    public static DynamicObject? ExecuteWithEntityFramework(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new DefaultEntityFrameworkExpressionExecutor(queryableProvider, context, setTypeInformation).Execute(expression);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>The result of the query execution.</returns>
    [SecuritySafeCritical]
    public static TResult ExecuteWithEntityFramework<TResult>(
        this Expression expression,
        DbContext dbContext,
        IExpressionTranslatorContext? context = null)
        => new CastingEntityFrameworkExpressionExecutor<TResult>(dbContext, context).Execute(expression);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>The result of the query execution.</returns>
    public static TResult ExecuteWithEntityFramework<TResult>(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        IExpressionTranslatorContext? context = null)
        => new CastingEntityFrameworkExpressionExecutor<TResult>(queryableProvider, context).Execute(expression);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>The mapped result of the query execution.</returns>
    [SecuritySafeCritical]
    public static ValueTask<DynamicObject?> ExecuteWithEntityFrameworkAsync(
        this Expression expression,
        DbContext dbContext,
        CancellationToken cancellation = default,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new DefaultEntityFrameworkExpressionExecutor(dbContext, context, setTypeInformation).ExecuteAsync(expression, cancellation);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/> and maps the result into dynamic objects.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <param name="setTypeInformation">Function to define whether to add type information.</param>
    /// <returns>The mapped result of the query execution.</returns>
    public static ValueTask<DynamicObject?> ExecuteWithEntityFrameworkAsync(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        CancellationToken cancellation = default,
        IExpressionTranslatorContext? context = null,
        Func<Type, bool>? setTypeInformation = null)
        => new DefaultEntityFrameworkExpressionExecutor(queryableProvider, context, setTypeInformation).ExecuteAsync(expression, cancellation);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="dbContext">Instance of <see cref="DbContext"/> to get the <see cref="DbSet{T}"/>.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>The result of the query execution.</returns>
    [SecuritySafeCritical]
    public static ValueTask<TResult> ExecuteWithEntityFrameworkAsync<TResult>(
        this Expression expression,
        DbContext dbContext,
        CancellationToken cancellation = default,
        IExpressionTranslatorContext? context = null)
        => new CastingEntityFrameworkExpressionExecutor<TResult>(dbContext, context).ExecuteAsync(expression, cancellation);

    /// <summary>
    /// Composes and executes the query based on the <see cref="Expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
    /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
    /// <param name="cancellation">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <param name="context">Optional instance of <see cref="IExpressionTranslatorContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
    /// <returns>The result of the query execution.</returns>
    public static ValueTask<TResult> ExecuteWithEntityFrameworkAsync<TResult>(
        this Expression expression,
        Func<Type, IQueryable> queryableProvider,
        CancellationToken cancellation = default,
        IExpressionTranslatorContext? context = null)
        => new CastingEntityFrameworkExpressionExecutor<TResult>(queryableProvider, context).ExecuteAsync(expression, cancellation);
}