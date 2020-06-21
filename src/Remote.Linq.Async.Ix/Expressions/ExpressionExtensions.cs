// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        private static CastingExpressionExecutor<TResult> CreateCastExecutor<TResult>(Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            ////=> new CastingExpressionExecutor<TResult>(asyncEnumerableProvider, typeResolver, canBeEvaluatedLocally);
            => throw new NotImplementedException();

        private static DefaultExpressionExecutor CreateDefaultExecutor(Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver, IDynamicObjectMapper? mapper, Func<Type, bool>? setTypeInformation, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            ////=> new DefaultExpressionExecutor(asyncEnumerableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally);
            => throw new NotImplementedException();

        /// <summary>
        /// Creates a <see cref="DefaultExpressionExecutionContext" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncEnumerableProvider">Delegate to provide <see cref="object"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>A new instance <see cref="AsyncDefaultExpressionExecutionContext" />.</returns>
        public static DefaultExpressionExecutionContext Executor(this Expression expression, Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new DefaultExpressionExecutionContext(CreateDefaultExecutor(asyncEnumerableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally), expression);

        /// <summary>
        /// Creates a <see cref="ExpressionExecutionContext{TResult}" /> for the given <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncEnumerableProvider">Delegate to provide <see cref="object"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>A new instance <see cref="ExpressionExecutionContext{TResult}" />.</returns>
        public static ExpressionExecutionContext<TResult> Executor<TResult>(this Expression expression, Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => new ExpressionExecutionContext<TResult>(CreateCastExecutor<TResult>(asyncEnumerableProvider, typeResolver, canBeEvaluatedLocally), expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncEnumerableProvider">Delegate to provide <see cref="object"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/>.</param>
        /// <param name="setTypeInformation">Function to define whether to add type information.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static IEnumerable<DynamicObject?>? Execute(this Expression expression, Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver = null, IDynamicObjectMapper? mapper = null, Func<Type, bool>? setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateDefaultExecutor(asyncEnumerableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <param name="asyncEnumerableProvider">Delegate to provide <see cref="object"/> instances for given <see cref="Type"/>s.</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects.</param>
        /// <param name="canBeEvaluatedLocally">Function to define which expressions may be evaluated locally, and which need to be retained for execution on the data source.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public static TResult Execute<TResult>(this Expression expression, Func<Type, object> asyncEnumerableProvider, ITypeResolver? typeResolver = null, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally = null)
            => CreateCastExecutor<TResult>(asyncEnumerableProvider, typeResolver, canBeEvaluatedLocally).Execute(expression);
    }
}
