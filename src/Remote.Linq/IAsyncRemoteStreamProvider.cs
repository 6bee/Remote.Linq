// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;

    public interface IAsyncRemoteStreamProvider : IRemoteQueryProvider
    {
        /// <summary>
        /// Executes the remote query represented by the specified expression tree.
        /// </summary>
        /// <returns>An <see cref="IAsyncEnumerable{T}"/> representing the remote stream.</returns>
        IAsyncEnumerable<T> ExecuteAsyncRemoteStream<T>(Expression expression, CancellationToken cancellation);
    }
}
