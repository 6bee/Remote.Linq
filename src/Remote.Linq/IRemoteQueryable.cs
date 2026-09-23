// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for remote execution.
/// </summary>
public interface IRemoteQueryable : IRemoteLinqQueryable, IQueryable
{
    /// <summary>
    /// Gets the expression tree that is associated with the instance of <see cref="IRemoteQueryable"/>.
    /// </summary>
    new Expression Expression { get; } // hides both IRemoteLinqQueryable.Expression and IQueryable.Expression to resolve ambiguity

    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    new IRemoteQueryProvider Provider { get; }
}
