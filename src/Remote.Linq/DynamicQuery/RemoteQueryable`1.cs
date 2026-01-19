// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for remote execution.
/// </summary>
public class RemoteQueryable<T>(IRemoteQueryProvider provider, Expression? expression = null)
    : RemoteQueryable(typeof(T), provider, expression), IOrderedRemoteQueryable<T>
{
    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
        => Execute().GetEnumerator();

    /// <inheritdoc/>
    public IEnumerable<T> Execute()
        => Provider.Execute<IEnumerable<T>>(Expression);
}