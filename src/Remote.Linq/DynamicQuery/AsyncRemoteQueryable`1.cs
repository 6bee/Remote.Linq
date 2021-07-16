// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncRemoteQueryable<T> : AsyncRemoteQueryable, IOrderedAsyncRemoteQueryable<T>
    {
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
