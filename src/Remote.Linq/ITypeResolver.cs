// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq
{
    public interface ITypeResolver
    {
        Type ResolveType(string typeName);
    }
}
