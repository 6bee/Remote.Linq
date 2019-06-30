// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class ObjectResultCaster : IQueryResultMapper<object>
    {
        public TResult MapResult<TResult>(object source, Expression expression)
        {
            var sourceType = source?.GetType();
            if (sourceType?.IsArray ?? false)
            {
                var elementType = TypeHelper.GetElementType(sourceType);
                if (typeof(TResult).IsAssignableFrom(elementType))
                {
                    try
                    {
                        if ((expression as MethodCallExpression)?.Arguments.Count == 2)
                        {
                            var predicate = GetTruePredicate(elementType);
                            source = MethodInfos.Enumerable.SingleWithFilter.MakeGenericMethod(elementType).Invoke(null, new object[] { source, predicate });
                        }
                        else
                        {
                            source = MethodInfos.Enumerable.Single.MakeGenericMethod(elementType).Invoke(null, new object[] { source });
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

            return (TResult)source;
        }

        private static object GetTruePredicate(Type t)
            => Expression.Lambda(Expression.Constant(true), Expression.Parameter(t)).Compile();
    }
}
