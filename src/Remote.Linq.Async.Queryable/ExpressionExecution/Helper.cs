// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    internal static class Helper
    {
        private const BindingFlags PrivateStatic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly MethodInfo _toSingleElementStreamMethodInfo = typeof(Helper).GetMethod(nameof(ToSingleElementStream), PrivateStatic);
        private static readonly MethodInfo _mapAsyncEnumerableMethodInfo = typeof(Helper).GetMethod(nameof(MapAsyncEnumerableInternal), PrivateStatic);
        private static readonly MethodInfo _mapTaskAsyncMethodInfo = typeof(Helper).GetMethod(nameof(MapAsyncResultInternal), PrivateStatic);

        public static IAsyncEnumerable<object?> TaskResultToSingleElementStream(object task, Type resultType)
            => (IAsyncEnumerable<object?>)_toSingleElementStreamMethodInfo
            .MakeGenericMethod(resultType)
            .Invoke(null, new[] { task });

        private static async IAsyncEnumerable<object?> ToSingleElementStream<T>(Task<T> task)
        {
            var result = await task.ConfigureAwait(false);
            yield return result;
        }

        public static IAsyncEnumerable<object?> MapAsyncEnumerable(object asyncEnumerable, Type itemType)
            => (IAsyncEnumerable<object?>)_mapAsyncEnumerableMethodInfo
            .MakeGenericMethod(itemType)
            .Invoke(null, new[] { asyncEnumerable });

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
            var method = _mapTaskAsyncMethodInfo.MakeGenericMethod(resultType);
            return (ValueTask<object?>)method.Invoke(null, new[] { task });
        }

        private static async ValueTask<object?> MapAsyncResultInternal<T>(Task<T> task)
            => await task.ConfigureAwait(false);
    }
}