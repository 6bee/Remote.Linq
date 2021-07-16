// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// A <i>Remote.Linq</i> queryable for async remote execution.
    /// </summary>
    public class RemoteLinqAsyncQueryable : IRemoteLinqQueryable, IOrderedAsyncQueryable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLinqAsyncQueryable"/> class.
        /// </summary>
        public RemoteLinqAsyncQueryable(Type elementType, IRemoteLinqAsyncQueryProvider provider, Expression? expression)
        {
            ElementType = elementType.CheckNotNull(nameof(elementType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        /// <inheritdoc/>
        public Type ElementType { get; }

        /// <inheritdoc/>
        public Expression Expression { get; }

        /// <inheritdoc/>
        public IAsyncQueryProvider Provider { get; }

        /// <inheritdoc/>
        Type IRemoteLinqQueryable.ResourceType => ElementType;
    }
}