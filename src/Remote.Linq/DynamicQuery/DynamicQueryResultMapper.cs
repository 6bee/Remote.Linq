// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem.Extensions;
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public class DynamicQueryResultMapper : DynamicObjectMapper
    {
        private static readonly Func<Type[], MethodInfo> _MapGroupToDynamicObjectGraphMethod = genericTypeArguments =>
             typeof(DynamicQueryResultMapper)
                 .GetMethod(nameof(MapGroupToDynamicObjectGraph), BindingFlags.NonPublic | BindingFlags.Instance)
                 .MakeGenericMethod(genericTypeArguments);

        protected override bool ShouldMapToDynamicObject(IEnumerable collection)
            => collection.GetType().Implements(typeof(IGrouping<,>));

        protected override DynamicObject MapToDynamicObjectGraph(object obj, Func<Type, bool> setTypeInformation)
        {
            Type[] genericTypeArguments = null;
            if (obj?.GetType().Implements(typeof(IGrouping<,>), out genericTypeArguments) ?? false)
            {
                return (DynamicObject)_MapGroupToDynamicObjectGraphMethod(genericTypeArguments)
                    .Invoke(this, new[] { obj, setTypeInformation });
            }

            return base.MapToDynamicObjectGraph(obj, setTypeInformation);
        }

        private DynamicObject MapGroupToDynamicObjectGraph<TKey, TElement>(IGrouping<TKey, TElement> group, Func<Type, bool> setTypeInformation)
        {
            var remoteLinqGroup = (group as Grouping<TKey, TElement>) ??
                new Grouping<TKey, TElement>
                {
                    Key = group.Key,
                    Elements = group.ToArray(),
                };

            return base.MapToDynamicObjectGraph(remoteLinqGroup, setTypeInformation);
        }
    }
}
