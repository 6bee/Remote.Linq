// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Extensions;
    using Aqua.TypeSystem;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    public sealed class ObjectResultCaster : IQueryResultMapper<object?>
    {
        [return: MaybeNull]
        public TResult MapResult<TResult>(object? source, Expression expression)
        {
            if (source != null && !(source is TResult) && source.IsCollection(out var enumerable))
            {
                var elementType = TypeHelper.GetElementType(enumerable.GetType()) ?? throw new RemoteLinqException($"Failed to get element type of {enumerable.GetType()}.");
                if (typeof(TResult).IsAssignableFrom(elementType) && expression is MethodCallExpression methodCallExpression)
                {
                    return DynamicResultMapper.MapToSingleResult<TResult>(elementType, enumerable, methodCallExpression);
                }
            }

            return (TResult)source;
        }
    }
}
