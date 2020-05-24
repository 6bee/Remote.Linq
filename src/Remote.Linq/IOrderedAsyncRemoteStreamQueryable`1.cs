// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq
{
    using System.Linq;

    public interface IOrderedAsyncRemoteStreamQueryable<out T> : IAsyncRemoteStreamQueryable<T>, IOrderedQueryable<T>
    {
    }
}

#endif // ASYNC_STREAM
