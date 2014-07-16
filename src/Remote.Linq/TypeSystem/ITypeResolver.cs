// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;

namespace Remote.Linq.TypeSystem
{
    public interface ITypeResolver
    {
        Type ResolveType(TypeInfo typeInfo);
    }
}
