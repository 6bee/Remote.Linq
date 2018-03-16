// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.ComponentModel;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryFunctions
    {
        public static IQueryable<T> Include<T>(IQueryable<T> queryable, string path)
            => queryable;
    }
}
