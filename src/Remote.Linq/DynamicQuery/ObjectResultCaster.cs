// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.EnumerableExtensions;
    using Aqua.TypeSystem;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    public sealed class ObjectResultCaster : IQueryResultMapper<object?>
    {
        [return: MaybeNull]
        public TResult MapResult<TResult>(object? source, Expression expression)
        {
            if (source is not null && source is not TResult && source.IsCollection(out var enumerable))
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
