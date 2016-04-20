// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Linq;

    // not publicly exposed in order to reduce complexity of API
    internal static class ProviderRegistry
    {
        public static Func<Type, IQueryable> QueryableResourceProvider { get; set; }
    }
}
