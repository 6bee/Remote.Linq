// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.ExpressionExecution
{
    using Aqua.Dynamic;
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MethodInfo = System.Reflection.MethodInfo;
    using SystemLinq = System.Linq.Expressions;

    /// <summary>
    /// Context for translating async query expressions from <i>Remote.Linq</i> to <i>System.Linq</i>.
    /// </summary>
    public class AsyncQueryExpressionTranslatorContext : ExpressionTranslatorContext
    {
        protected class DynamicAsyncQueryResultObjectMapper : ExpressionTranslatorContextObjectMapper
        {
            public DynamicAsyncQueryResultObjectMapper(ExpressionTranslatorContext dynamicQueryResultMapper)
                : base(dynamicQueryResultMapper)
            {
            }

            public DynamicAsyncQueryResultObjectMapper(ITypeResolver typeResolver, ITypeInfoProvider typeInfoProvider, IIsKnownTypeProvider isKnownTypeProvider)
                : base(typeResolver, typeInfoProvider, isKnownTypeProvider)
            {
            }

            protected override bool ShouldMapToDynamicObject(IEnumerable collection)
                => collection.CheckNotNull().GetType().Implements(typeof(IAsyncGrouping<,>))
                || base.ShouldMapToDynamicObject(collection);

            protected override DynamicObject? MapToDynamicObjectGraph(object? obj, Func<Type, bool> setTypeInformation)
            {
                var genericTypeArguments = default(Type[]);
                if (obj?.GetType().Implements(typeof(IAsyncGrouping<,>), out genericTypeArguments) is true)
                {
                    obj = MapAsyncGroupToDynamicObjectGraphMethod(genericTypeArguments!).Invoke(null, new[] { obj });
                }
                else if (obj?.GetType().Implements(typeof(IAsyncEnumerable<>), out genericTypeArguments) is true)
                {
                    obj = MapAsyncEnumerableMethod(genericTypeArguments!).Invoke(null, new[] { obj });
                }

                return base.MapToDynamicObjectGraph(obj, setTypeInformation);
            }
        }

        private static readonly MethodInfo _mapAsyncGroupToDynamicObjectGraph = typeof(AsyncQueryExpressionTranslatorContext).GetMethodEx(nameof(MapAsyncGroupToDynamicObjectGraph));
        private static readonly MethodInfo _mapAsyncEnumerable = typeof(AsyncQueryExpressionTranslatorContext).GetMethodEx(nameof(MapAsyncEnumerable));

        private static readonly Func<Type[], MethodInfo> MapAsyncGroupToDynamicObjectGraphMethod =
            genericArguments => _mapAsyncGroupToDynamicObjectGraph.MakeGenericMethod(genericArguments);

        private static readonly Func<Type[], MethodInfo> MapAsyncEnumerableMethod =
            genericArguments => _mapAsyncEnumerable.MakeGenericMethod(genericArguments);

        public AsyncQueryExpressionTranslatorContext(
            ITypeResolver? typeResolver = null,
            ITypeInfoProvider? typeInfoProvider = null,
            IIsKnownTypeProvider? isKnownTypeProvider = null,
            Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally = null)
            : base(typeResolver, typeInfoProvider, isKnownTypeProvider, canBeEvaluatedLocally)
            => ValueMapper = new DynamicAsyncQueryResultObjectMapper(this);

        public override IDynamicObjectMapper ValueMapper { get; }

        private static AsyncEnumerable<T> MapAsyncEnumerable<T>(IAsyncEnumerable<T> source)
            => source is AsyncEnumerable<T> asyncEnumerable
            ? asyncEnumerable
            : new AsyncEnumerable<T> { Elements = EnumerateBlocking(source).ToArray() };

        private static AsyncGrouping<TKey, TElement> MapAsyncGroupToDynamicObjectGraph<TKey, TElement>(IAsyncGrouping<TKey, TElement> group)
            => group is AsyncGrouping<TKey, TElement> remoteLinqGroup
            ? remoteLinqGroup
            : new AsyncGrouping<TKey, TElement> { Key = group.Key, Elements = EnumerateBlocking(group).ToArray() };

        private static IEnumerable<T> EnumerateBlocking<T>(IAsyncEnumerable<T> source)
        {
            if (source is IEnumerable<T> elements)
            {
                return elements;
            }

            var list = new List<T>();

            Task.Run(async () =>
                {
                    await foreach (var item in source)
                    {
                        list.Add(item);
                    }
                })
                .Wait();

            return list;
        }
    }
}