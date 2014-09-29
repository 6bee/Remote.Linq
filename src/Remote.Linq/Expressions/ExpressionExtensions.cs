// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Remote.Linq.Expressions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        public static IEnumerable<DynamicObject> Execute(this Expression expression, Func<Type, IQueryable> queryableProvider, ITypeResolver typeResolver = null, Func<IDynamicObjectMapper> mapper = null)
        {
            var queryableExpression = expression.ReplaceResourceDescriptorsByQueryable(typeResolver, queryableProvider);
            var linqExpression = queryableExpression.ToLinqExpression();

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
            catch (TargetInvocationException ex)
            {
                var excption = ex.InnerException;

                if (excption is InvalidOperationException && string.Compare(excption.Message, "Sequence contains no elements") == 0)
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

            if (ReferenceEquals(null, mapper))
            {
                mapper = () => new DynamicObjectMapper { SuppressDynamicTypeInformation = true };
            }

            var dynamicObjects = mapper().MapCollection(queryResult);
            return dynamicObjects;
        }
    }
}
