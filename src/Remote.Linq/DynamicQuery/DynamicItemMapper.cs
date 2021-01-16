// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class DynamicItemMapper : IAsyncQueryResultMapper<DynamicObject?>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicItemMapper(IDynamicObjectMapper? mapper)
        {
            _mapper = mapper ?? new DynamicObjectMapper();
        }

        public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellationToken)
#pragma warning disable CS8604 // Possible null reference argument.
            => new ValueTask<TResult>(source is null ? default : _mapper.Map<TResult>(source));
#pragma warning restore CS8604 // Possible null reference argument.
    }
}
