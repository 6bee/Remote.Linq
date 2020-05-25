// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    public interface IAsyncRemoteStreamQueryable : IRemoteQueryable
    {
        new IAsyncRemoteStreamProvider Provider { get; }
    }
}
