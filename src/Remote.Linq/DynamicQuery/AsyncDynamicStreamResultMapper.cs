// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Aqua.Dynamic;
using System.Linq.Expressions;

/// <summary>
/// Asynchronous query result mapper for async streams.
/// </summary>
public sealed class AsyncDynamicStreamResultMapper : IAsyncQueryResultMapper<DynamicObject>
{
    private readonly IDynamicObjectMapper? _mapper;

    public AsyncDynamicStreamResultMapper(IDynamicObjectMapper? mapper = null)
        => _mapper = mapper;

    /// <inheritdoc/>
    public ValueTask<TResult> MapResultAsync<TResult>(DynamicObject? source, Expression expression, CancellationToken cancellation = default)
    {
        var mapper = _mapper ?? ExpressionTranslatorContext.Default.ValueMapper;
        return new(mapper.Map<TResult>(source));
    }
}