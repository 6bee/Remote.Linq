// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery;

using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

/// <summary>
/// Provides functionality to compose queries for remote execution.
/// </summary>
public class RemoteQueryable : IOrderedRemoteQueryable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteQueryable"/> class.
    /// </summary>
    public RemoteQueryable(Type elementType, IRemoteQueryProvider provider, Expression? expression = null)
    {
        ElementType = elementType.CheckNotNull();
        Provider = provider.CheckNotNull();
        Expression = expression ?? Expression.Constant(this);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
        => Provider.Execute(Expression) is IEnumerable enumerable
        ? enumerable.GetEnumerator()
        : throw new RemoteLinqException($"Expression execution did not return an {typeof(IEnumerable)}");

    /// <inheritdoc/>
    public Type ElementType { get; }

    /// <inheritdoc/>
    public Expression Expression { get; }

    /// <inheritdoc/>
    public IRemoteQueryProvider Provider { get; }

    /// <inheritdoc/>
    IQueryProvider IQueryable.Provider => Provider;

    /// <inheritdoc/>
    Type IRemoteLinqQueryable.ResourceType => ElementType;
}