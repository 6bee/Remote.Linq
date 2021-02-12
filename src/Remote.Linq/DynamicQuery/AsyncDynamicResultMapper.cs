// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class AsyncDynamicResultMapper : IAsyncQueryResultMapper<DynamicObject>
    {
        private readonly IDynamicObjectMapper? _mapper;

        public AsyncDynamicResultMapper(IDynamicObjectMapper? mapper)
        {
            _mapper = mapper;
        }

        public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellation = default)
#pragma warning disable CS8604 // Possible null reference argument.
            => new ValueTask<TResult>(DynamicResultMapper.MapToType<TResult>(source, _mapper, expression));
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
