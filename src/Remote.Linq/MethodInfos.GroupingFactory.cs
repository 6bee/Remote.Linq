// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static partial class MethodInfos
    {
        internal static class GroupingFactory
        {
            internal static readonly MethodInfo MapMany = GetMethod(typeof(GroupingFactory), nameof(InternalMapMany));
            internal static readonly MethodInfo MapOne = GetMethod(typeof(GroupingFactory), nameof(InternalMapOne));

            private static IEnumerable<Grouping<TKey, TElement>> InternalMapMany<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> list)
                => list.Select(InternalMapOne).ToArray();

            private static Grouping<TKey, TElement> InternalMapOne<TKey, TElement>(IGrouping<TKey, TElement> grouping)
                => new Grouping<TKey, TElement> { Key = grouping.Key, Elements = grouping.ToArray() };
        }
    }
}