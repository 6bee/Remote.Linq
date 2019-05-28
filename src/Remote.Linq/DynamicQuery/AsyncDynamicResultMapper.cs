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
        private readonly IDynamicObjectMapper _mapper;

        public AsyncDynamicResultMapper(IDynamicObjectMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<TResult> MapResultAsync<TResult>(IEnumerable<DynamicObject> source, Expression expression, CancellationToken cancellationToken)
            => Task.Run(() => DynamicResultMapper.MapToType<TResult>(source, _mapper, expression), cancellationToken);
    }
}
