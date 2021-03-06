﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;

    public interface IExpressionFromRemoteLinqContext : IExpressionValueMapperProvider
    {
        ITypeResolver TypeResolver { get; }
    }
}