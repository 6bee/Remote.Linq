// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

using Remote.Linq.DynamicQuery;
using System.Linq.Expressions;

/// <summary>
/// A <i>Remote.Linq</i> queryable for async remote execution of EntityFrameworkCore queries.
/// </summary>
public class RemoteLinqEfCoreAsyncQueryable<T>(IRemoteLinqEfCoreAsyncQueryProvider provider, Expression? expression) : AsyncRemoteQueryable<T>(provider, expression), IAsyncEnumerable<T>
{
    /// <inheritdoc/>
    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var result = ((IRemoteLinqEfCoreAsyncQueryProvider)Provider).ExecuteAsyncRemoteStream<T>(Expression, cancellationToken).ConfigureAwait(false);
        await foreach (var item in result.WithCancellation(cancellationToken))
        {
            yield return item;
        }
    }
}