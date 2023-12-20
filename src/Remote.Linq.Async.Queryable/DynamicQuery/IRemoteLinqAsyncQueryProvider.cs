// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery;

using System.Linq;

/// <summary>
/// Represents a query provider for <i>Remote.Linq</i> version of asynchronous queryable sequences.
/// </summary>
public interface IRemoteLinqAsyncQueryProvider : IAsyncQueryProvider, IRemoteLinqQueryProvider
{
}