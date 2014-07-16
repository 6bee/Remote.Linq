// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;

namespace Remote.Linq
{
    internal static class ProviderRegistry
    {
        public static Func<Type, IQueryable> QueryableResourceProvider { get; set; }
    }
}
