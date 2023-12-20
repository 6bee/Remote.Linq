﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

/// <summary>
/// Represents the result of a sorting operation of a remote stream queryable resource.
/// </summary>
public interface IOrderedAsyncRemoteStreamQueryable : IAsyncRemoteStreamQueryable, IOrderedRemoteQueryable
{
}