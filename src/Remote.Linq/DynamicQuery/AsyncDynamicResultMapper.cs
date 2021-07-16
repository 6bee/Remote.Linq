// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Asynchronous query result mapper.
    /// </summary>
    public sealed class AsyncDynamicResultMapper : IAsyncQueryResultMapper<DynamicObject>
    {
        private readonly IDynamicObjectMapper? _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncDynamicResultMapper"/> class.
        /// </summary>
        public AsyncDynamicResultMapper(IDynamicObjectMapper? mapper)
            => _mapper = mapper;

        /// <inheritdoc/>
        public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellation = default)
            => new ValueTask<TResult>(DynamicResultMapper.MapToType<TResult>(source, _mapper, expression) !);
    }
}
