// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncRemoteQueryProvider : IRemoteQueryProvider
    {
        /// <summary>
        /// Executes the remote query represented by the specified expression tree.
        /// </summary>
        /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous result of the remote query.</returns>
        ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellation);
    }
}