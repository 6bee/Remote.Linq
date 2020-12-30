// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
<<<<<<< HEAD
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
=======
>>>>>>> 6.3.x
    using System.Linq;

    /// <summary>
    ///   <para>
    ///     Provides query extension methods that can be used to compose query expressions.
    ///   </para>
    ///   <para>
    ///     The functions do not provide any logic themselves but rather act as placeholders
    ///     and may be replaced by corresponding implementations before query execution,
    ///     given such implementations exists with the given linq provider.
    ///   </para>
    /// </summary>
    public static class QueryFunctions
    {
        [SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Path argument needed as expression template")]
        public static IQueryable<T> Include<T>(IQueryable<T> queryable, string path) => queryable;
    }
}
