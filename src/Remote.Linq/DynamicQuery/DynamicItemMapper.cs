﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class DynamicItemMapper : IAsyncQueryResultMapper<DynamicObject>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicItemMapper(IDynamicObjectMapper? mapper)
            => _mapper = mapper ?? new DynamicQueryResultMapper().ValueMapper;

        public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellation = default)
            => new ValueTask<TResult>(_mapper.Map<TResult>(source));
    }
}