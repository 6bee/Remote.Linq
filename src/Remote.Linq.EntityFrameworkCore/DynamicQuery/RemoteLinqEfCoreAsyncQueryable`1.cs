// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

using Remote.Linq.DynamicQuery;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A <i>Remote.Linq</i> queryable for async remote execution of EntityFrameworkCore queries.
/// </summary>
public class RemoteLinqEfCoreAsyncQueryable<T> : AsyncRemoteQueryable<T>, IAsyncEnumerable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteLinqEfCoreAsyncQueryable{T}"/> class.
    /// </summary>
    public RemoteLinqEfCoreAsyncQueryable(IRemoteLinqEfCoreAsyncQueryProvider provider, Expression? expression)
        : base(provider, expression)
    {
    }

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