// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#if !NET35

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAsyncRemoteQueryable<T> : IAsyncRemoteQueryable, IRemoteQueryable<T>
    {
        Task<IEnumerable<T>> ExecuteAsync();
    }

    public interface IOrderedAsyncRemoteQueryable<T> : IAsyncRemoteQueryable<T>, IOrderedAsyncRemoteQueryable, IOrderedRemoteQueryable<T>
    {
    }
}

#endif