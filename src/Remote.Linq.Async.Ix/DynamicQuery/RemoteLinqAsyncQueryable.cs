// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class RemoteLinqAsyncQueryable : IRemoteResource, IOrderedAsyncQueryable
    {
        protected RemoteLinqAsyncQueryable(Type elementType, IAsyncQueryProvider provider, Expression? expression)
        {
            ElementType = elementType.CheckNotNull(nameof(elementType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IAsyncQueryProvider Provider { get; }

        Type IRemoteResource.ResourceType => ElementType;
    }
}
