// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Remote.Linq.Expressions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        public static IEnumerable<DynamicObject> Execute(this Remote.Linq.Expressions.Expression expression, Func<Type, System.Linq.IQueryable> queryableProvider)
        {
            var queryableExpression = expression.ReplaceResourceDescriptorsByQueryable(queryableProvider);
            var linqExpression = queryableExpression.ToLinqExpression();

            //System.Diagnostics.Debug.WriteLine("Remote expression: {0}", queryableExpression);
            //System.Diagnostics.Debug.WriteLine("System expression: {0}", linqExpression);

            var lambdaExpression = linqExpression as System.Linq.Expressions.LambdaExpression;
            if (lambdaExpression == null)
            {
                lambdaExpression = System.Linq.Expressions.Expression.Lambda(linqExpression);
            }

            object queryable;
            try
            {
                queryable = lambdaExpression.Compile().DynamicInvoke();
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                var excption = ex.InnerException;

                System.Diagnostics.Debug.WriteLine(excption);

                if (excption is System.InvalidOperationException && string.Compare(excption.Message, "Sequence contains no elements") == 0)
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
            var dynamicObjects = DynamicObjectMapper.Map(queryResult, suppressTypeInformation: true);
            return dynamicObjects;
        }
    }
}
