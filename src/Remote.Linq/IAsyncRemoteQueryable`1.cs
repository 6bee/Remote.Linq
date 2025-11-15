// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Provides functionality to compose queries for async remote execution.
/// </summary>
public interface IAsyncRemoteQueryable<T> : IAsyncRemoteQueryable, IRemoteQueryable<T>
{
    /// <summary>
    /// Executes the remote queryable.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.ValueTask`1"/> representing the asynchronous result of the remote query.</returns>
    ValueTask<IEnumerable<T>> ExecuteAsync(CancellationToken cancellation = default);
}