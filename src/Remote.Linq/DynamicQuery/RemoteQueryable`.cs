// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq.DynamicQuery
{
    internal sealed partial class RemoteQueryable<T> : RemoteQueryable, IQueryable<T>
    {
        internal RemoteQueryable(IQueryProvider provider)
            : base(typeof(T), provider)
        {
        }

        internal RemoteQueryable(IQueryProvider provider, Expression expression)
            : base(typeof(T), provider, expression)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
        }
    }
}
