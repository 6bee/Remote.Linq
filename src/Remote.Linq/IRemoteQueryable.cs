// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;

    /// <summary>
    /// Provides functionality to compose queries for remote execution.
    /// </summary>
    public interface IRemoteQueryable : IRemoteLinqQueryable, IQueryable
    {
        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        new IRemoteQueryProvider Provider { get; }
    }
}