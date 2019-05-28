// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncRemoteQueryable<T> : IAsyncRemoteQueryable, IRemoteQueryable<T>
    {
        Task<IEnumerable<T>> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
