// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal sealed class AsyncRemoteQueryable<T> : RemoteQueryable, IAsyncRemoteQueryable<T>, IOrderedAsyncRemoteQueryable<T>
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
            => Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();

        Task<IEnumerable<T>> IAsyncRemoteQueryable<T>.ExecuteAsync()
            => ((IAsyncRemoteQueryProvider)Provider).ExecuteAsync<IEnumerable<T>>(Expression);
    }
}
