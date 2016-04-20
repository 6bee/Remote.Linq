// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Executes the <see cref="System.Linq.Expressions.Expression"/> and returns the raw result
        /// </summary>
        /// <param name="expression">The <see cref="System.Linq.Expressions.Expression"/> to be executed</param>
        /// <returns>Execution result of the <see cref="System.Linq.Expressions.Expression"/> specified</returns>
        public static object Execute(this System.Linq.Expressions.Expression expression)
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

                if (excption is InvalidOperationException && string.Equals(excption.Message, "Sequence contains no elements", StringComparison.Ordinal))
                {
                    return new DynamicObject[0];
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
                var isQueryable = queryableType.IsGenericType() && queryableType.GetInterfaces().Any(x => x.IsGenericType() && x.GetGenericTypeDefinition() == typeof(IQueryable<>));
                if (isQueryable)
                {
                    // force query execution
                    try
                    {
                        var elementType = TypeHelper.GetElementType(queryableType);
                        queryResult = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).Invoke(null, new[] { queryable });
                    }
                    catch (System.Reflection.TargetInvocationException ex)
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
        /// Composes and executes the query based on the <see cref="Expression"/> and mappes the result into dynamic objects
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped result of the query execution</returns>
        public static IEnumerable<DynamicObject> Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
        {
            var linqExpression = PrepareForExecution(expression, queryableProvider, typeResolver);

            var queryResult = Execute(linqExpression);

            var dynamicObjects = ConvertResultToDynamicObjects(queryResult, mapper, setTypeInformation);
            return dynamicObjects;
        }

        /// <summary>
        /// Converts the query result into a collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="queryResult">The reult of the query execution</param>
        /// <param name="mapper">Optional instance of <see cref="IDynamicObjectMapper"/></param>
        /// <returns>The mapped query result</returns>
        public static IEnumerable<DynamicObject> ConvertResultToDynamicObjects(object queryResult, IDynamicObjectMapper mapper = null, Func<Type, bool> setTypeInformation = null)
        {
            if (ReferenceEquals(null, mapper))
            {
                mapper = new DynamicObjectMapper();
            }

            if (ReferenceEquals(null, setTypeInformation))
            {
                setTypeInformation = t => !t.IsAnonymousType();
            }

            var dynamicObjects = mapper.MapCollection(queryResult, setTypeInformation);
            return dynamicObjects;
        }

        /// <summary>
        /// Prepares the query <see cref="Expression"/> to be able to be executed. <para/> 
        /// Use this method if you wan to execute the <see cref="System.Linq.Expressions.Expression"/> and map the raw result yourself.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> to be executed</param>
        /// <param name="queryableProvider">Delegate to provide <see cref="IQueryable"/> instances based on <see cref="Type"/>s</param>
        /// <param name="typeResolver">Optional instance of <see cref="ITypeResolver"/> to be used to translate <see cref="Aqua.TypeSystem.TypeInfo"/> into <see cref="Type"/> objects</param>
        /// <returns>A <see cref="System.Linq.Expressions.Expression"/> ready for execution</returns>
        public static System.Linq.Expressions.Expression PrepareForExecution(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null)
        {
            var expression1 = expression.ReplaceNonGenericQueryArgumentsByGenericArguments();

            var queryableExpression = expression1.ReplaceResourceDescriptorsByQueryable(typeResolver, queryableProvider);

            var linqExpression = queryableExpression.ToLinqExpression(typeResolver);

            var locallyEvaluatedExpression = linqExpression.PartialEval();
            return locallyEvaluatedExpression;
        }
    }
}
