// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.DynamicQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;

    internal sealed class RemoteLinqAsyncQueryable<T> : RemoteLinqAsyncQueryable, IOrderedAsyncQueryable<T>
    {
        internal RemoteLinqAsyncQueryable(IAsyncQueryProvider provider, Expression? expression = null)
            : base(typeof(T), provider, expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => Provider is IRemoteLinqAsyncQueryProvider remoteLinqAsyncQueryProvider
            ? remoteLinqAsyncQueryProvider.GetAsyncEnumerator<T>(Expression, cancellationToken)
            : throw new InvalidOperationException($"Remote async stream requires a remote.linq query provider to execute.");
    }
}
