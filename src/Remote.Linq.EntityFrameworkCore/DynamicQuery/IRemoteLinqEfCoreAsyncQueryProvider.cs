// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.DynamicQuery;

#if NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
using IAsyncQueryProvider = Microsoft.EntityFrameworkCore.Query.IAsyncQueryProvider;
#else
using IAsyncQueryProvider = Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider;
#endif

/// <summary>
/// Represents a query provider for <i>Remote.Linq</i> version of asynchronous queryable sequences.
/// </summary>
public interface IRemoteLinqEfCoreAsyncQueryProvider : IAsyncQueryProvider, IAsyncRemoteStreamProvider, IAsyncRemoteQueryProvider, IRemoteQueryProvider
{
}