// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Reflection;

    internal static partial class MethodInfos
    {
        internal static class QueryFunction
        {
            internal static readonly MethodInfo Include = GetMethod(
                typeof(Remote.Linq.DynamicQuery.QueryFunctions),
                nameof(Remote.Linq.DynamicQuery.QueryFunctions.Include));
        }
    }
}