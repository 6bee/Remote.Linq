// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for remote execution as async stream.
/// </summary>
public sealed class AsyncRemoteStreamQueryable<T>(IAsyncRemoteStreamProvider provider, Expression? expression = null)
    : AsyncRemoteStreamQueryable(typeof(T), provider, expression), IOrderedAsyncRemoteStreamQueryable<T>
{
    /// <inheritdoc/>
    public IAsyncEnumerable<T> ExecuteAsyncRemoteStream(CancellationToken cancellation = default)
        => Provider.ExecuteAsyncRemoteStream<T>(Expression, cancellation);

    /// <summary>
    /// This operation must not be used on stream queryable.
    /// </summary>
    /// <exception cref="NotSupportedException">Always thrown for stream queryable.</exception>
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => throw QueryOperationNotSupportedException;
}