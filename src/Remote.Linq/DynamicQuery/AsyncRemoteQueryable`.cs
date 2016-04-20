// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if !NET35

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal sealed partial class AsyncRemoteQueryable<T> : RemoteQueryable, IAsyncRemoteQueryable<T>, IOrderedAsyncRemoteQueryable<T>
    {
        internal AsyncRemoteQueryable(IAsyncRemoteQueryProvider provider)
            : base(typeof(T), provider)
        {
        }

        internal AsyncRemoteQueryable(IAsyncRemoteQueryProvider provider, Expression expression)
            : base(typeof(T), provider, expression)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (_provider.Execute<IEnumerable<T>>(_expression)).GetEnumerator();
        }

        public Task<IEnumerable<T>> ExecuteAsync()
        {
            return ((IAsyncRemoteQueryProvider)_provider).ExecuteAsync<IEnumerable<T>>(_expression);
        }
    }
}

#endif