// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides functionality to compose queries for async remote execution.
    /// </summary>
    public sealed class AsyncRemoteQueryable<T> : AsyncRemoteQueryable, IOrderedAsyncRemoteQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRemoteQueryable{T}"/> class.
        /// </summary>
        public AsyncRemoteQueryable(IAsyncRemoteQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        /// <inheritdoc/>
        public ValueTask<IEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default)
            => Provider.ExecuteAsync<IEnumerable<T>>(Expression, cancellation);

        /// <inheritdoc/>
        public IEnumerable<T> Execute()
            => Provider.Execute<IEnumerable<T>>(Expression);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
            => Execute().GetEnumerator();
    }
}