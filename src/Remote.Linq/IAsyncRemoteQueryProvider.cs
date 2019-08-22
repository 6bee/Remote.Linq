// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAsyncRemoteQueryProvider : IRemoteQueryProvider
    {
        Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
    }
}
