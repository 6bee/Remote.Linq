// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides functionality to compose queries for remote execution.
    /// </summary>
    public interface IRemoteQueryable<out T> : IRemoteQueryable, IQueryable<T>
    {
        /// <summary>
        /// Executes the remote queryable and returns the result.
        /// </summary>
        /// <returns>The result of the remote queryable.</returns>
        IEnumerable<T> Execute();
    }
}