// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using Remote.Linq.Async;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for remote execution as async stream.
/// </summary>
public class AsyncRemoteStreamQueryable : IOrderedAsyncRemoteStreamQueryable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRemoteStreamQueryable"/> class.
    /// </summary>
    public AsyncRemoteStreamQueryable(Type elementType, IAsyncRemoteStreamProvider provider, Expression? expression)
    {
        ElementType = elementType.CheckNotNull();
        Provider = provider.CheckNotNull();
        Expression = expression ?? Expression.Constant(this);
    }

    /// <inheritdoc/>
    public Type ElementType { get; }

    /// <inheritdoc/>
    public Expression Expression { get; }

    /// <inheritdoc/>
    public IAsyncRemoteStreamProvider Provider { get; }

    /// <inheritdoc/>
    IRemoteQueryProvider IRemoteQueryable.Provider => Provider;

    /// <inheritdoc/>
    IQueryProvider IQueryable.Provider => Provider;

    /// <summary>
    /// This operation must not be used on stream queryable.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown for stream queryable.</exception>
    IEnumerator IEnumerable.GetEnumerator() => throw QueryOperationNotSupportedException;

    /// <inheritdoc/>
    Type IRemoteLinqQueryable.ResourceType => ElementType;

    protected internal static Exception QueryOperationNotSupportedException
        => new NotSupportedException(
            "Async remote stream must be executed as IAsyncEnumerable<T>. " +
            $"The {nameof(AsyncQueryableExtensions.AsAsyncEnumerable)}() extension method may be used.");
}