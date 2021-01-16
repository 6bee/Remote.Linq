// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class AsyncDynamicResultMapper : IAsyncQueryResultMapper<IEnumerable<DynamicObject>>
    {
        private readonly IDynamicObjectMapper? _mapper;

        public AsyncDynamicResultMapper(IDynamicObjectMapper? mapper)
        {
            _mapper = mapper;
        }

#nullable disable
        public ValueTask<TResult> MapResultAsync<TResult>(IEnumerable<DynamicObject> source, Expression expression, CancellationToken cancellation = default)
            => new ValueTask<TResult>(DynamicResultMapper.MapToType<TResult>(source, _mapper, expression));
#nullable restore
    }
}
