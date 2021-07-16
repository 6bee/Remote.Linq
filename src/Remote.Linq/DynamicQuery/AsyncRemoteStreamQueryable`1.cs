// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;

    /// <summary>
    /// Provides functionality to compose queries for remote execution as async stream.
    /// </summary>
    public sealed class AsyncRemoteStreamQueryable<T> : AsyncRemoteStreamQueryable, IOrderedAsyncRemoteStreamQueryable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRemoteStreamQueryable{T}"/> class.
        /// </summary>
        public AsyncRemoteStreamQueryable(IAsyncRemoteStreamProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<T> ExecuteAsyncRemoteStream(CancellationToken cancellation = default)
            => Provider.ExecuteAsyncRemoteStream<T>(Expression, cancellation);

        /// <summary>
        /// This operation must not be used on stream queryable.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown for stream queryable.</exception>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => throw QueryOperationNotSupportedException;
    }
}