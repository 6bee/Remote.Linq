// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;

    public interface IRemoteQueryable : IRemoteResource, IQueryable
    {
        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        new IRemoteQueryProvider Provider { get; }
    }
}
