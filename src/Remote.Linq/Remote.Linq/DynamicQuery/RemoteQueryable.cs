// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Remote.Linq.Dynamic;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    internal partial class RemoteQueryable : IRemoteQueryable, IOrderedRemoteQueryable
    {
        protected readonly Type _elemntType;
        protected readonly Expression _expression;
        protected readonly IRemoteQueryProvider _provider;

        internal RemoteQueryable(Type elemntType, IRemoteQueryProvider provider)
        {
            _elemntType = elemntType;
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        internal RemoteQueryable(Type elemntType, IRemoteQueryProvider provider, Expression expression)
        {
            _elemntType = elemntType;
            _provider = provider;
            _expression = expression;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_provider.Execute<System.Collections.IEnumerable>(_expression)).GetEnumerator();
        }

        Type IQueryable.ElementType { get { return _elemntType; } }

        Expression IQueryable.Expression { get { return _expression; } }

        IQueryProvider IQueryable.Provider { get { return _provider; } }
    }
}
