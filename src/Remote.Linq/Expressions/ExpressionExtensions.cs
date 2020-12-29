// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionExecution;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        private static CastingExpressionExecutor<TQueryable, TResult> CreateCastExecutor<TQueryable, TResult>(Func<Type, TQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            => new CastingExpressionExecutor<TQueryable, TResult>(queryableProvider, typeResolver, canBeEvaluatedLocally);

        private static DefaultExpressionExecutor CreateDefaultExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver, IDynamicObjectMapper? mapper, Func<Type, bool>? setTypeInformation, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            => new DefaultExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally);

        /// <summary>
        /// Creates a <see cref="DefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
        public static DefaultExpressionExecutionContext Executor(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new DefaultExpressionExecutionContext(CreateDefaultExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally), expression);

        /// <summary>
        /// Creates a <see cref="ExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>A new instance <see cref="ExpressionExecutionContext{TResult}" />.</returns>
        public static ExpressionExecutionContext<TResult> Executor<TResult>(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new ExpressionExecutionContext<TResult>(CreateCastExecutor<IQueryable, TResult>(queryableProvider, typeResolver, canBeEvaluatedLocally), expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static IEnumerable<DynamicObject?>? Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateDefaultExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static TResult Execute<TResult>(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateCastExecutor<IQueryable, TResult>(queryableProvider, typeResolver, canBeEvaluatedLocally).Execute(expression);
    }
}
