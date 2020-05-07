// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Linq;

    public static class RemoteQueryable
    {
        /// <summary>
        /// Gets a factory for creating <see cref="IQueryable{T}"/>
        /// (or <see cref="IQueryable"/> respectively) suited for remote execution.
        /// </summary>
        public static RemoteQueryableFactory Factory { get; } = new RemoteQueryableFactory();
    }
}
