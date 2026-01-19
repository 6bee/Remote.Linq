// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for async remote execution.
/// </summary>
public class AsyncRemoteQueryable(Type elementType, IAsyncRemoteQueryProvider provider, Expression? expression = null)
    : RemoteQueryable(elementType, provider, expression), IOrderedAsyncRemoteQueryable
{
    /// <inheritdoc/>
    public new IAsyncRemoteQueryProvider Provider => (IAsyncRemoteQueryProvider)base.Provider;
}