// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;

    internal sealed class AsyncRemoteStreamQueryable<T> : AsyncRemoteStreamQueryable, IAsyncRemoteStreamQueryable<T>, IOrderedAsyncRemoteStreamQueryable<T>
    {
        internal AsyncRemoteStreamQueryable(IAsyncRemoteStreamProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        public IAsyncEnumerable<T> ExecuteAsyncRemoteStream(CancellationToken cancellation) => Provider.ExecuteAsyncRemoteStream<T>(Expression, cancellation);

        public IEnumerator<T> GetEnumerator() => throw QueryOperationNotSupportedException;
    }
}

#endif // ASYNC_STREAM
