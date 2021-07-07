// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class RemoteLinqAsyncQueryable : IRemoteLinqQueryable, IOrderedAsyncQueryable
    {
        public RemoteLinqAsyncQueryable(Type elementType, IRemoteLinqAsyncQueryProvider provider, Expression? expression)
        {
            ElementType = elementType.CheckNotNull(nameof(elementType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IAsyncQueryProvider Provider { get; }

        Type IRemoteLinqQueryable.ResourceType => ElementType;
    }
}