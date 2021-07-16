// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;

    public class RemoteQueryable : IOrderedRemoteQueryable
    {
        public RemoteQueryable(Type elemntType, IRemoteQueryProvider provider, Expression? expression = null)
        {
            ElementType = elemntType.CheckNotNull(nameof(elemntType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => Provider.Execute(Expression) is IEnumerable enumerable
            ? enumerable.GetEnumerator()
            : throw new RemoteLinqException($"Expression execution did not return an {typeof(IEnumerable)}");

        /// <inheritdoc />
        public Type ElementType { get; }

        /// <inheritdoc />
        public Expression Expression { get; }

        /// <inheritdoc />
        public IRemoteQueryProvider Provider { get; }

        /// <inheritdoc />
        IQueryProvider IQueryable.Provider => Provider;

        /// <inheritdoc />
        Type IRemoteLinqQueryable.ResourceType => ElementType;
    }
}
