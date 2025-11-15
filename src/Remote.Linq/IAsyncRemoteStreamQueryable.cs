// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Provides functionality to compose queries for remote execution as async stream.
/// </summary>
public interface IAsyncRemoteStreamQueryable : IQueryable, IRemoteLinqQueryable
{
    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    new IAsyncRemoteStreamProvider Provider { get; }
}