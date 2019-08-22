// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class AsyncRemoteQueryable<T> : RemoteQueryable<T>, IAsyncRemoteQueryable<T>, IOrderedAsyncRemoteQueryable<T>
    {
        internal AsyncRemoteQueryable(IAsyncRemoteQueryProvider provider)
            : base(provider)
        {
        }

        internal AsyncRemoteQueryable(IAsyncRemoteQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }

        public new IAsyncRemoteQueryProvider Provider => (IAsyncRemoteQueryProvider)base.Provider;

        public Task<IEnumerable<T>> ExecuteAsync(CancellationToken cancellationToken)
            => Provider.ExecuteAsync<IEnumerable<T>>(Expression, cancellationToken);
    }
}
