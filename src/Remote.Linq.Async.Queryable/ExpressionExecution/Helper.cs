// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.TypeExtensions;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    internal static class Helper
    {
        private static readonly MethodInfo _toSingleElementStreamMethod = typeof(Helper).GetMethodEx(nameof(ToSingleElementStream));
        private static readonly MethodInfo _mapAsyncEnumerableMethod = typeof(Helper).GetMethodEx(nameof(MapAsyncEnumerableInternal));
        private static readonly MethodInfo _mapTaskAsyncMethod = typeof(Helper).GetMethodEx(nameof(MapAsyncResultInternal));

        private static readonly Func<Type, MethodInfo> ToSingleElementStreamMethod = args => _toSingleElementStreamMethod.MakeGenericMethod(args);
        private static readonly Func<Type, MethodInfo> MapAsyncEnumerableMethod = args => _mapAsyncEnumerableMethod.MakeGenericMethod(args);
        private static readonly Func<Type, MethodInfo> MapTaskAsyncMethod = args => _mapTaskAsyncMethod.MakeGenericMethod(args);

        public static IAsyncEnumerable<object?> TaskResultToSingleElementStream(object task, Type resultType)
        {
            task.AssertNotNull();
            resultType.AssertNotNull();
            var result = ToSingleElementStreamMethod(resultType).Invoke(null, new[] { task });
            return (IAsyncEnumerable<object?>)result!;
        }

        private static async IAsyncEnumerable<object?> ToSingleElementStream<T>(Task<T> task)
        {
            task.AssertNotNull();
            var result = await task.ConfigureAwait(false);
            yield return result;
        }

        public static IAsyncEnumerable<object?> MapAsyncEnumerable(object asyncEnumerable, Type itemType)
        {
            itemType.AssertNotNull();
            var result = MapAsyncEnumerableMethod(itemType).Invoke(null, new[] { asyncEnumerable });
            return (IAsyncEnumerable<object?>)result!;
        }

        private static async IAsyncEnumerable<object?> MapAsyncEnumerableInternal<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            if (asyncEnumerable is not null)
            {
                await foreach (var item in asyncEnumerable.ConfigureAwait(false))
                {
                    yield return item;
                }
            }
        }

        public static ValueTask<object?> MapTaskResultAsync(object task, Type resultType)
        {
            task.AssertNotNull();
            resultType.AssertNotNull();
            var result = MapTaskAsyncMethod(resultType).Invoke(null, new[] { task });
            return (ValueTask<object?>)result!;
        }

        private static async ValueTask<object?> MapAsyncResultInternal<T>(Task<T> task)
            => await task.ConfigureAwait(false);
    }
}