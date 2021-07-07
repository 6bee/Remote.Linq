// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;

    public interface IAsyncRemoteStreamQueryable : IQueryable, IRemoteLinqQueryable
    {
        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        new IAsyncRemoteStreamProvider Provider { get; }
    }
}