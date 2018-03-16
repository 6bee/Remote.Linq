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
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

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
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <returns>The mapped result of the query execution</returns>
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
        /// Prepares the <see cref="Expression"/> befor being transformed<para/>
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/></param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/></returns>
        protected virtual Expression Prepare(Expression expression)
        {
            var expression1 = expression.ReplaceNonGenericQueryArgumentsByGenericArguments();
            var queryableExpression = expression1.ReplaceResourceDescriptorsByQueryable(_typeResolver, _queryableProvider);
            return queryableExpression;
        }

        /// <summary>
        /// Transforms the <see cref="Expression"/> to a <see cref="System.Linq.Expressions.Expression"/>
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be transformed</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/></returns>
        protected virtual System.Linq.Expressions.Expression Transform(Expression expression)
        {
            var linqExpression = expression.ToLinqExpression(_typeResolver);
            return linqExpression;
        }

        /// <summary>
        /// Prepares the query <see cref="System.Linq.Expressions.Expression"/> to be able to be executed.
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> returned by the Transform method.</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        protected virtual System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
        {
            var locallyEvaluatedExpression = expression.PartialEval(_canBeEvaluatedLocally);
            return locallyEvaluatedExpression;
        }

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified</returns>
        protected virtual object Execute(System.Linq.Expressions.Expression expression)
        {
            var lambdaExpression = expression as System.Linq.Expressions.LambdaExpression;
            if (lambdaExpression == null)
            {
                lambdaExpression = System.Linq.Expressions.Expression.Lambda(expression);
            }

            object queryable;
            try
            {
                queryable = lambdaExpression.Compile().DynamicInvoke();
            }
            catch (TargetInvocationException ex)
            {
                var excption = ex.InnerException;

                if (excption is InvalidOperationException)
                {
                    if (string.Equals(excption.Message, "Sequence contains no elements", StringComparison.Ordinal))
                    {
                        return new DynamicObject[0];
                    }

                    if (string.Equals(excption.Message, "Sequence contains no matching element", StringComparison.Ordinal))
                    {
                        return new DynamicObject[0];
                    }

                    if (string.Equals(excption.Message, "Sequence contains more than one element", StringComparison.Ordinal))
                    {
                        return new DynamicObject[2];
                    }

                    if (string.Equals(excption.Message, "Sequence contains more than one matching element", StringComparison.Ordinal))
                    {
                        return new DynamicObject[2];
                    }
                }

                throw excption;
            }

            object queryResult;
            if (ReferenceEquals(null, queryable))
            {
                queryResult = null;
            }
            else
            {
                var queryableType = queryable.GetType();
                var isQueryable = queryableType.Implements(typeof(IQueryable<>));
                if (isQueryable)
                {
                    // force query execution
                    try
                    {
                        var elementType = TypeHelper.GetElementType(queryableType);
                        queryResult = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).Invoke(null, new[] { queryable });
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
                else
                {
                    queryResult = queryable;
                }
            }

            return queryResult;
        }

        /// <summary>
        /// If overriden in a derived transforms the collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="queryResult">The reult of the query execution</param>
        /// <returns>Processed result</returns>
        protected virtual object ProcessResult(object queryResult)
            => queryResult;

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="queryResult">The reult of the query execution</param>
        /// <returns>The mapped query result</returns>
        protected virtual IEnumerable<DynamicObject> ConvertResult(object queryResult)
        {
            var dynamicObjects = _mapper.MapCollection(queryResult, _setTypeInformation);
            return dynamicObjects;
        }

        /// <summary>
        /// If overriden in a derived transforms the collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="queryResult">The reult of the query execution</param>
        /// <returns>Processed result</returns>
        protected virtual IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
            => queryResult;

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
