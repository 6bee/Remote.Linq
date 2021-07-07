// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class RemoteLinqAsyncQueryable<T> : RemoteLinqAsyncQueryable, IOrderedAsyncQueryable<T>
    {
        public RemoteLinqAsyncQueryable(IRemoteLinqAsyncQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        public ValueTask<IAsyncEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default)
            => ((IRemoteLinqAsyncQueryProvider)Provider).ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellation);

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            var result = await ExecuteAsync(cancellationToken).ConfigureAwait(false);
            await foreach (var item in result.WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}