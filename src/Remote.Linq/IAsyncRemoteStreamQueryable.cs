// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if ASYNC_STREAM

namespace Remote.Linq
{
    public interface IAsyncRemoteStreamQueryable : IRemoteQueryable
    {
        new IAsyncRemoteStreamProvider Provider { get; }
    }
}

#endif // ASYNC_STREAM
