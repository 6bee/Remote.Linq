// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;

    public sealed class AsyncRemoteStreamQueryable<T> : AsyncRemoteStreamQueryable, IOrderedAsyncRemoteStreamQueryable<T>
    {
        public AsyncRemoteStreamQueryable(IAsyncRemoteStreamProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        public IAsyncEnumerable<T> ExecuteAsyncRemoteStream(CancellationToken cancellation = default) => Provider.ExecuteAsyncRemoteStream<T>(Expression, cancellation);

        public IEnumerator<T> GetEnumerator() => throw QueryOperationNotSupportedException;
    }
}
