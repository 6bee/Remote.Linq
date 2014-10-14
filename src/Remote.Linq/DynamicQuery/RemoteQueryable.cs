// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq.DynamicQuery
{
    internal partial class RemoteQueryable : IQueryable
    {
        protected readonly Type _elemntType;
        protected readonly Expression _expression;
        protected readonly IQueryProvider _provider;

        internal RemoteQueryable(Type elemntType, IQueryProvider provider)
        {
            _elemntType = elemntType;
            _provider = provider;
            _expression = Expression.Constant(this);
        }

        internal RemoteQueryable(Type elemntType, IQueryProvider provider, Expression expression)
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
