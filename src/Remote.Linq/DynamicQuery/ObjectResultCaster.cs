// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class ObjectResultCaster : IQueryResultMapper<object>
    {
        public TResult MapResult<TResult>(object source, Expression expression)
        {
            if (source is IEnumerable enumerable)
            {
                var elementType = TypeHelper.GetElementType(enumerable.GetType());
                if (typeof(TResult).IsAssignableFrom(elementType) && expression is MethodCallExpression methodCallExpression)
                {
                    return DynamicResultMapper.MapToSingleResult<TResult>(elementType, enumerable, methodCallExpression);
                }
            }

            return (TResult)source;
        }
    }
}
