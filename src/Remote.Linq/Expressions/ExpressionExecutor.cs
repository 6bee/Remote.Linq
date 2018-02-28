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
    
    public class ExpressionExecutor
    {
        private readonly Func<Type, IQueryable> _queryableProvider;
        private readonly ITypeResolver _typeResolver;
        private readonly IDynamicObjectMapper _mapper;
        private readonly Func<Type, bool> _setTypeInformation;
        private readonly Func<System.Linq.Expressions.Expression, bool> _canBeEvaluatedLocally;

        /// <summary>
        /// Creates a new instance of <see cref="ExpressionExecutor"/>
        /// </summary>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
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
        public virtual IEnumerable<DynamicObject> Execute(Expression expression)
        {
            var linqExpression = PrepareForExecution(expression);
            var queryResult = Execute(linqExpression);            
            var dynamicObjects = ConvertResultToDynamicObjects(queryResult);
            return dynamicObjects;
        }

        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified</returns>
        protected internal virtual object Execute(System.Linq.Expressions.Expression expression)
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
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        protected internal virtual System.Linq.Expressions.Expression PrepareForExecution(Expression expression)
        {
            var expression1 = expression.ReplaceNonGenericQueryArgumentsByGenericArguments();

            var queryableExpression = expression1.ReplaceResourceDescriptorsByQueryable(_typeResolver, _queryableProvider);

            var linqExpression = queryableExpression.ToLinqExpression(_typeResolver);

            var locallyEvaluatedExpression = linqExpression.PartialEval(_canBeEvaluatedLocally);
            return locallyEvaluatedExpression;
        }

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="queryResult">The reult of the query execution</param>
        /// <returns>The mapped query result</returns>
        protected internal virtual IEnumerable<DynamicObject> ConvertResultToDynamicObjects(object queryResult)
        {
            var dynamicObjects = _mapper.MapCollection(queryResult, _setTypeInformation);
            return dynamicObjects;
        }
    }
}
