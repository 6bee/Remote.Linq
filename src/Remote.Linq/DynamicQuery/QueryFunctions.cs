// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QueryFunctions
    {
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Path argument needed as expression template")]
        public static IQueryable<T> Include<T>(IQueryable<T> queryable, string path) => queryable;
    }
}
