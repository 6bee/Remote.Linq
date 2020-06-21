// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;

    internal class AsyncRemoteStreamQueryable : IOrderedAsyncRemoteStreamQueryable
    {
        internal AsyncRemoteStreamQueryable(Type elementType, IAsyncRemoteStreamProvider provider, Expression? expression)
        {
            ElementType = elementType.CheckNotNull(nameof(elementType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IAsyncRemoteStreamProvider Provider { get; }

        IRemoteQueryProvider IRemoteQueryable.Provider => Provider;

        IQueryProvider IQueryable.Provider => Provider;

        IEnumerator IEnumerable.GetEnumerator() => throw QueryOperationNotSupportedException;

        Type IRemoteResource.ResourceType => ElementType;

        internal static Exception QueryOperationNotSupportedException
            => new InvalidOperationException($"Async remote stream must be executed as IAsyncEnumerable<T>. The {nameof(AsyncQueryableExtensions.AsAsyncEnumerable)}() extension method may be used.");
   }
}
