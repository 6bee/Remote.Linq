// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq
{
    internal sealed class RemoteQueryable<T> : RemoteQueryable, IQueryable<T>
    {
        internal RemoteQueryable(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
            :base(new QueryableDescriptor<T>(), dataProvider)
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
