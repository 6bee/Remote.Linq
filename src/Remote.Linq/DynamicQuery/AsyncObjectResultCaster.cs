// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class AsyncObjectResultCaster : IAsyncQueryResultMapper<object>
    {
        public Task<TResult> MapResultAsync<TResult>(object source, Expression expression, CancellationToken cancellationToken)
            => Task.FromResult((TResult)source);
    }
}
