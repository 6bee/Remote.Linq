// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public class DynamicQueryResultMapper : DynamicObjectMapper
    {
        private static readonly Func<Type[], MethodInfo> _MapGroupToDynamicObjectGraphMethod = genericTypeArguments =>
             typeof(DynamicQueryResultMapper)
                 .GetMethod(nameof(MapGroupToDynamicObjectGraph), BindingFlags.NonPublic | BindingFlags.Static)
                 .MakeGenericMethod(genericTypeArguments);

        protected override bool ShouldMapToDynamicObject(IEnumerable collection)
            => collection.CheckNotNull(nameof(collection)).GetType().Implements(typeof(IGrouping<,>))
            || base.ShouldMapToDynamicObject(collection);

        protected override DynamicObject? MapToDynamicObjectGraph(object? obj, Func<Type, bool> setTypeInformation)
        {
            var genericTypeArguments = default(Type[]);
            if (obj?.GetType().Implements(typeof(IGrouping<,>), out genericTypeArguments) is true)
            {
                obj = _MapGroupToDynamicObjectGraphMethod(genericTypeArguments!).Invoke(null, new[] { obj });
            }

            return base.MapToDynamicObjectGraph(obj, setTypeInformation);
        }

        private static Grouping<TKey, TElement> MapGroupToDynamicObjectGraph<TKey, TElement>(IGrouping<TKey, TElement> group)
            => (group as Grouping<TKey, TElement>) ??
                new Grouping<TKey, TElement>
                {
                    Key = group.Key,
                    Elements = group.ToArray(),
                };
    }
}
