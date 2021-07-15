// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.Expressions;
    using System;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionExtensions
    {
        private static CastingExpressionExecutor<TQueryable, TResult> CreateCastExecutor<TQueryable, TResult>(Func<Type, TQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context)
            => new CastingExpressionExecutor<TQueryable, TResult>(queryableProvider, context);

        private static DefaultExpressionExecutor CreateDefaultExecutor(Func<Type, IQueryable> queryableProvider, IExpressionFromRemoteLinqContext? context, Func<Type, bool>? setTypeInformation)
            => new DefaultExpressionExecutor(queryableProvider, context, setTypeInformation);

        /// <summary>
        /// Creates a <see cref="DefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
        public static DefaultExpressionExecutionContext Executor(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => new DefaultExpressionExecutionContext(CreateDefaultExecutor(queryableProvider, context, setTypeInformation), expression);

        /// <summary>
        /// Creates a <see cref="ExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>A new instance <see cref="ExpressionExecutionContext{TResult}" />.</returns>
        public static ExpressionExecutionContext<TResult> Executor<TResult>(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null)
            => new ExpressionExecutionContext<TResult>(CreateCastExecutor<IQueryable, TResult>(queryableProvider, context), expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static DynamicObject Execute(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null,
            Func<Type, bool>? setTypeInformation = null)
            => CreateDefaultExecutor(queryableProvider, context, setTypeInformation).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="context">Optional instance of <see cref="IExpressionFromRemoteLinqContext"/> to be used to translate <see cref="TypeInfo"/>, <see cref="Expression"/> etc.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static TResult Execute<TResult>(
            this Expression expression,
            Func<Type, IQueryable> queryableProvider,
            IExpressionFromRemoteLinqContext? context = null)
            => CreateCastExecutor<IQueryable, TResult>(queryableProvider, context).Execute(expression);
    }
}