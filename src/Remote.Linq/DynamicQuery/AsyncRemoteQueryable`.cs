// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Remote.Linq.DynamicQuery
{
    internal sealed partial class AsyncRemoteQueryable<T> : RemoteQueryable, IAsyncQueryable<T>
    {
        internal AsyncRemoteQueryable(IAsyncQueryProvider provider)
            : base(typeof(T), provider)
        {
        }

        internal AsyncRemoteQueryable(IAsyncQueryProvider provider, Expression expression)
            : base(typeof(T), provider, expression)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
        }

        public Task<IEnumerable<T>> ExecuteAsync()
        {
            return ((IAsyncQueryProvider)_provider).ExecuteAsync<IEnumerable<T>>(_expression);
        }
    }
}
