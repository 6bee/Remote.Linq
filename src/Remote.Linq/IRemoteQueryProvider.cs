// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Represents a query provider for <i>Remote.Linq</i> version of queryable sequences.
/// </summary>
public interface IRemoteQueryProvider : IQueryProvider, IRemoteLinqQueryProvider;