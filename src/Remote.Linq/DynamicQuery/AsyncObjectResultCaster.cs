// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncObjectResultCaster : IAsyncQueryResultMapper<object>
    {
        private readonly ObjectResultCaster _objectResultCaster = new ObjectResultCaster();

        public ValueTask<TResult> MapResultAsync<TResult>(object? source, Expression expression, CancellationToken cancellation = default)
            => new ValueTask<TResult>(_objectResultCaster.MapResult<TResult>(source, expression) !);
    }
}
