// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    internal sealed class CastingReactiveAsyncExpressionExecutor<TResult> : ReactiveAsyncExpressionExecutor<TResult>
    {
        public CastingReactiveAsyncExpressionExecutor(Func<Type, IAsyncQueryable> queryableProvider, ITypeResolver? typeResolver, Func<System.Linq.Expressions.Expression, bool>? canBeEvaluatedLocally)
            : base(queryableProvider, typeResolver, canBeEvaluatedLocally)
        {
        }

        protected override TResult ConvertResult(object? queryResult)
        {
            if (queryResult is null)
            {
                return default!;
            }

            if (queryResult is TResult result)
            {
                return result;
            }

            if (typeof(TResult).Implements(typeof(IAsyncEnumerable<>), out var asyncEnumerableElementType))
            {
                if (queryResult.GetType().Implements(typeof(IEnumerable<>), out var elementType))
                {
                    var sourceType = elementType[0];
                    var targetType = asyncEnumerableElementType[0];
                    if (!targetType.IsAssignableFrom(sourceType))
                    {
                        throw new InvalidCastException($"Cannot cast elements of type '{sourceType}' to type '{targetType}'.");
                    }

                    return CallMapAsAsyncEnumerable(queryResult, sourceType, targetType);
                }
            }

            throw new NotImplementedException(nameof(ConvertResult));
        }

        private static readonly System.Reflection.MethodInfo _mapAsAsyncEnumerableMethodInfo =
            typeof(CastingReactiveAsyncExpressionExecutor<TResult>).GetMethod(nameof(MapAsAsyncEnumerable), BindingFlags.NonPublic | BindingFlags.Static);

        private static TResult CallMapAsAsyncEnumerable(object collection, Type sourceElementType, Type targetElementType)
        {
            var method = _mapAsAsyncEnumerableMethodInfo.MakeGenericMethod(sourceElementType, targetElementType);
            var result = method.Invoke(null, new[] { collection });
            return (TResult)result;
        }

        private static async IAsyncEnumerable<TTarget> MapAsAsyncEnumerable<TSource, TTarget>(IEnumerable<TSource> source)
        {
            await foreach (var item in source.ToAsyncEnumerable().ConfigureAwait(false))
            {
                yield return (TTarget)(object)item!;
            }
        }
    }
}