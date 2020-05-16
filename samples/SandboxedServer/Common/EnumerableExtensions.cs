// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}
