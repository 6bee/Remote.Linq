// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Asynchronous query result mapper for async streams.
    /// </summary>
    public sealed class AsyncDynamicStreamResultMapper : IAsyncQueryResultMapper<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;

        public AsyncDynamicStreamResultMapper(IDynamicObjectMapper? mapper)
            => _mapper = mapper ?? new DynamicQueryResultMapper().ValueMapper;

        /// <inheritdoc/>
        public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellation = default)
            => new ValueTask<TResult>(_mapper.Map<TResult>(source));
    }
}