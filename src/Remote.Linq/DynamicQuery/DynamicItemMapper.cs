// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;

    internal sealed class DynamicItemMapper : IQueryResultMapper<DynamicObject?>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicItemMapper(IDynamicObjectMapper? mapper)
        {
            _mapper = mapper ?? new DynamicObjectMapper();
        }

        [return: MaybeNull]
        public TResult MapResult<TResult>(DynamicObject? source, Expression expression)
            => source is null ? default : _mapper.Map<TResult>(source);
    }
}
