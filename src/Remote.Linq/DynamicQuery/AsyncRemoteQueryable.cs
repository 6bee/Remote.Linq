// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for async remote execution.
/// </summary>
public class AsyncRemoteQueryable : RemoteQueryable, IOrderedAsyncRemoteQueryable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRemoteQueryable"/> class.
    /// </summary>
    public AsyncRemoteQueryable(Type elementType, IAsyncRemoteQueryProvider provider, Expression? expression = null)
        : base(elementType, provider, expression)
    {
    }

    /// <inheritdoc/>
    public new IAsyncRemoteQueryProvider Provider => (IAsyncRemoteQueryProvider)base.Provider;
}