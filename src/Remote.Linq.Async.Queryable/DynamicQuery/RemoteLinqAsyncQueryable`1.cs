// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A <i>Remote.Linq</i> queryable for async remote execution.
    /// </summary>
    public class RemoteLinqAsyncQueryable<T> : RemoteLinqAsyncQueryable, IOrderedAsyncQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLinqAsyncQueryable{T}"/> class.
        /// </summary>
        public RemoteLinqAsyncQueryable(IRemoteLinqAsyncQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        /// <summary>
        /// Executes the remote queryable asynchronously.
        /// </summary>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the result of the async execution.</returns>
        public ValueTask<IAsyncEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default)
            => ((IRemoteLinqAsyncQueryProvider)Provider).ExecuteAsync<IAsyncEnumerable<T>>(Expression, cancellation);

        /// <summary>
        /// Gets an async stream to execute the remote queryable and enumerate the result.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An async stream of the query result.</returns>
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