// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;

    internal class RemoteQueryable : IOrderedRemoteQueryable
    {
        internal RemoteQueryable(Type elemntType, IRemoteQueryProvider provider, Expression? expression = null)
        {
            ElementType = elemntType.CheckNotNull(nameof(elemntType));
            Provider = provider.CheckNotNull(nameof(provider));
            Expression = expression ?? Expression.Constant(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();

        public Type ElementType { get; }

        public Expression Expression { get; }

        public IRemoteQueryProvider Provider { get; }

        IQueryProvider IQueryable.Provider => Provider;
    }
}
