﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;

    public interface IRemoteLinqQueryable
    {
        Type ResourceType { get; }
    }
}