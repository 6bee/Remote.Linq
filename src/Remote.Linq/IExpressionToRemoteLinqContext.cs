// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;

    public interface IExpressionToRemoteLinqContext : IExpressionValueMapperProvider
    {
        ITypeInfoProvider TypeInfoProvider { get; }

        Func<object, bool> NeedsMapping { get; }
    }
}