// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;

    internal sealed class RemoteLinqAsyncQueryable<T> : RemoteLinqAsyncQueryable, IOrderedAsyncQueryable<T>
    {
        internal RemoteLinqAsyncQueryable(IRemoteLinqAsyncQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var remoteLinqAsyncQueryProvider = (IRemoteLinqAsyncQueryProvider)Provider;
            var result = await remoteLinqAsyncQueryProvider.ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellationToken).ConfigureAwait(false);
            await foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
