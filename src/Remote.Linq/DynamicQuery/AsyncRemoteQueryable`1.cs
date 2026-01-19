// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for async remote execution.
/// </summary>
public class AsyncRemoteQueryable<T>(IAsyncRemoteQueryProvider provider, Expression? expression = null)
    : AsyncRemoteQueryable(typeof(T), provider, expression), IOrderedAsyncRemoteQueryable<T>
{
    /// <inheritdoc/>
    public ValueTask<IEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default)
        => Provider.ExecuteAsync<IEnumerable<T>>(Expression, cancellation);

    /// <inheritdoc/>
    public IEnumerable<T> Execute()
        => Provider.Execute<IEnumerable<T>>(Expression);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
        => Execute().GetEnumerator();
}