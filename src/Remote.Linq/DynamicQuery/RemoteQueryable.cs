// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal partial class RemoteQueryable : IRemoteQueryable, IOrderedRemoteQueryable
    {
        protected readonly Type _elemntType;
        protected readonly Expression _expression;
        protected readonly IRemoteQueryProvider _provider;

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
        {
            return (_provider.Execute<System.Collections.IEnumerable>(_expression)).GetEnumerator();
        }

        Type IQueryable.ElementType => _elemntType;

        Expression IQueryable.Expression => _expression;

        IQueryProvider IQueryable.Provider => _provider;
    }
}
