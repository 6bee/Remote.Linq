// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Represents the result of a sorting operation of a remote async queryable resource.
/// </summary>
public interface IOrderedAsyncRemoteQueryable : IAsyncRemoteQueryable, IOrderedRemoteQueryable;