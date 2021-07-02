// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Reflection;

    partial class MethodInfos
    {
        internal static class IncludeQueryFunctions
        {
            internal static readonly MethodInfo Include = GetMethod(
                typeof(Remote.Linq.Extensions.Include.IncludeQueryFunctions),
                nameof(Remote.Linq.Extensions.Include.IncludeQueryFunctions.Include));
        }
    }
}