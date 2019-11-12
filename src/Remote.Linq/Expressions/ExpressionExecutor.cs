// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security;

    public class ExpressionExecutor : IExpressionExecutionDecorator
    {
        private readonly Func<Type, IQueryable> _queryableProvider;
        private readonly ITypeResolver _typeResolver;
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;
        private readonly Func<System.Linq.Expressions.Expression, bool> _canBeEvaluatedLocally;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionExecutor"/> class.
        /// </summary>
        public ExpressionExecutor(Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null, Func<System.Linq.Expressions.Expression, bool> canBeEvaluatedLocally = null)
        {
            _queryableProvider = queryableProvider;
            _typeResolver = typeResolver;
            _mapper = mapper ?? new DynamicQueryResultMapper();
            _setTypeInformation = setTypeInformation ?? (t => !t.IsAnonymousType());
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        /// <summary>
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed.</param>
        /// <returns>The mapped result of the query execution.</returns>
        public IEnumerable<DynamicObject> Execute(Expression expression)
        {
            var preparedRemoteExpression = Prepare(expression);
            var linqExpression = Transform(preparedRemoteExpression);
            var preparedLinqExpression = Prepare(linqExpression);
            var queryResult = Execute(preparedLinqExpression);
            var processedResult = ProcessResult(queryResult);
            var dynamicObjects = ConvertResult(processedResult);
            var processedDynamicObjects = ProcessResult(dynamicObjects);
            return processedDynamicObjects;
        }

        /// <summary>
        /// Prepares the <see cref="Expression"/> befor being transformed.<para/>
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/>.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/>.</returns>
        protected virtual Expression Prepare(Expression expression)
        {
            var expression1 = expression.ReplaceNonGenericQueryArgumentsByGenericArguments();
            var queryableExpression = expression1.ReplaceResourceDescriptorsByQueryable(_typeResolver, _queryableProvider);
            return queryableExpression;
        }

        /// <summary>
        /// Transforms the <see cref="Expression"/> to a <see cref="System.Linq.Expressions.Expression"/>.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be transformed.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/>.</returns>
        protected virtual System.Linq.Expressions.Expression Transform(Expression expression)
        {
            var linqExpression = expression.ToLinqExpression(_typeResolver);
            return linqExpression;
        }

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution.</returns>
        protected virtual System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
        {
            var locallyEvaluatedExpression = expression.PartialEval(_canBeEvaluatedLocally);
            return locallyEvaluatedExpression;
        }

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
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected virtual object Execute(System.Linq.Expressions.Expression expression)
        {
            try
            {
                return ExecuteCore(expression);
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

                throw ex;
            }
        }

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed.</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified.</returns>
        protected static object ExecuteCore(System.Linq.Expressions.Expression expression)
        {
            var queryResult = CompileAndInvokeExpression(expression);
            if (queryResult is null)
            {
                return null;
            }

            var queryableType = queryResult.GetType();
            if (queryableType.Implements(typeof(IQueryable<>)))
            {
                // force query execution
                var elementType = TypeHelper.GetElementType(queryableType);
                queryResult = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).InvokeAndUnwrap(null, queryResult);
            }

            return queryResult;
        }

        [SecuritySafeCritical]
        protected static object CompileAndInvokeExpression(System.Linq.Expressions.Expression expression)
        {
            var lambdaExpression =
                (expression as System.Linq.Expressions.LambdaExpression) ??
                System.Linq.Expressions.Expression.Lambda(expression);
            return lambdaExpression.Compile().DynamicInvokeAndUnwrap();
        }

        /// <summary>
        /// If overriden in a derived transforms the collection of <see cref="DynamicObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual object ProcessResult(object queryResult)
            => queryResult;

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>The mapped query result.</returns>
        protected virtual IEnumerable<DynamicObject> ConvertResult(object queryResult)
            => _mapper.MapCollection(queryResult, _setTypeInformation);

        /// <summary>
        /// If overriden in a derived transforms the collection of <see cref="DynamicObject"/>.
        /// </summary>
        /// <param name="queryResult">The reult of the query execution.</param>
        /// <returns>Processed result.</returns>
        protected virtual IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
            => queryResult ?? Enumerable.Empty<DynamicObject>();

        Expression IExpressionExecutionDecorator.Prepare(Expression expression)
            => Prepare(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator.Transform(Expression expression)
            => Transform(expression);

        System.Linq.Expressions.Expression IExpressionExecutionDecorator.Prepare(System.Linq.Expressions.Expression expression)
            => Prepare(expression);

        object IExpressionExecutionDecorator.Execute(System.Linq.Expressions.Expression expression)
            => Execute(expression);

        object IExpressionExecutionDecorator.ProcessResult(object queryResult)
            => ProcessResult(queryResult);

        IEnumerable<DynamicObject> IExpressionExecutionDecorator.ConvertResult(object queryResult)
            => ConvertResult(queryResult);

        IEnumerable<DynamicObject> IExpressionExecutionDecorator.ProcessResult(IEnumerable<DynamicObject> queryResult)
            => ProcessResult(queryResult);
    }
}
