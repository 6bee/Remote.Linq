// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;

    public interface IAsyncRemoteStreamProvider : IRemoteQueryProvider
    {
        IAsyncEnumerable<T> ExecuteAsyncRemoteStream<T>(Expression expression, CancellationToken cancellation);
    }
}

#endif // ASYNC_STREAM
