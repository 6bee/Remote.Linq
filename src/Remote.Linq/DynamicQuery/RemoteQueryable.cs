// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal class RemoteQueryable : IRemoteQueryable, IOrderedRemoteQueryable
    {
        private readonly Type _elemntType;
        private readonly Expression _expression;
        private readonly IRemoteQueryProvider _provider;

        internal RemoteQueryable(Type elemntType, IRemoteQueryProvider provider)
            : this(elemntType, provider, null)
        {
        }

        internal RemoteQueryable(Type elemntType, IRemoteQueryProvider provider, Expression expression)
        {
            _elemntType = elemntType;
            _provider = provider;
            _expression = expression ?? Expression.Constant(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => _provider.Execute<System.Collections.IEnumerable>(_expression).GetEnumerator();

        public Type ElementType => _elemntType;

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;
    }
}
