// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Provides functionality to compose queries for remote execution as async stream.
    /// </summary>
    public interface IAsyncRemoteStreamQueryable<out T> : IQueryable<T>, IAsyncRemoteStreamQueryable
    {
        /// <summary>
        /// Executes the remote queryable and gets the result as async stream.
        /// </summary>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> providing the result as an async stream.</returns>
        IAsyncEnumerable<T> ExecuteAsyncRemoteStream(CancellationToken cancellation = default);
    }
}