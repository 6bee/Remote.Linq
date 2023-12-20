// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Provides functionality to compose queries for async remote execution.
/// </summary>
public interface IAsyncRemoteQueryable : IRemoteQueryable
{
    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    new IAsyncRemoteQueryProvider Provider { get; }
}