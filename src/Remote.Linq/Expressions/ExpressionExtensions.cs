// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        private static ExpressionExecutor CreateExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver, IDynamicObjectMapper mapper, Func<Type, bool> setTypeInformation, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally)
            => new ExpressionExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally);

        public static ExpressionExecutionContext Executor(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => new ExpressionExecutionContext(CreateExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally), expression);

        public static IEnumerable<DynamicObject> Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => CreateExecutor(queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).Execute(expression);

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        [Obsolete("This method will be removed in future versions. Inherit from Remote.Linq.Expressions.ExpressionExecutor or use expression.Executor(..).With(customstrategy).Execute() instead.", false)]
        public static object Execute(this System.Linq.Expressions.Expression expression)
            => ((IExpressionExecutionDecorator)new ExpressionExecutor(null)).Execute(expression);

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        [Obsolete("This method will be removed in future versions. In order to define custom result projection use expression.Executor(..).With(customstrategy).Execute() instead.", false)]
        public static IEnumerable<DynamicObject> Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver, Func<object, object> resultProjector, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
            => Executor(expression, queryableProvider, typeResolver, mapper, setTypeInformation, canBeEvaluatedLocally).With(resultProjector).Execute();

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>.
        /// </summary>
        [Obsolete("This method will be removed in future versions. Inherit from Remote.Linq.Expressions.ExpressionExecutor or use expression.Executor(..).With(customstrategy).Execute() instead.", false)]
        public static IEnumerable<DynamicObject> ConvertResultToDynamicObjects(object queryResult, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
        {
            var exec = (IExpressionExecutionDecorator)new ExpressionExecutor(null, null, mapper, setTypeInformation);
            return exec.ProcessResult(exec.ConvertResult(queryResult));
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/>
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        [Obsolete("This method will be removed in future versions. Inherit from Remote.Linq.Expressions.ExpressionExecutor or use expression.Executor(..).With(customstrategy).Execute() instead.", false)]
        public static System.Linq.Expressions.Expression PrepareForExecution(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            var exec = (IExpressionExecutionDecorator)new ExpressionExecutor(queryableProvider, typeResolver, canBeEvaluatedLocally: canBeEvaluatedLocally);
            return exec.Prepare(exec.Transform(exec.Prepare(expression)));
        }
    }
}
