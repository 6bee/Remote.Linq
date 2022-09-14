// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Denotes a <i>Remote.Linq</i> queryable resource.
    /// </summary>
    [QueryArgument]
    public interface IRemoteLinqQueryable
    {
        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="IRemoteLinqQueryable"/>.
        /// </summary>
        Expression Expression { get; }

        /// <summary>
        /// Gets the type of the remote queryable resource items.
        /// </summary>
        Type ResourceType { get; }
    }
}