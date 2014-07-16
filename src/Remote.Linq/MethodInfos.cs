// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using BindingFlags = System.Reflection.BindingFlags;
using MethodInfo = System.Reflection.MethodInfo;

namespace Remote.Linq
{
    internal static class MethodInfos
    {
        internal static class Enumerable
        {
            internal static readonly MethodInfo Cast = typeof(System.Linq.Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
            internal static readonly MethodInfo ToArray = typeof(System.Linq.Enumerable).GetMethod("ToArray", BindingFlags.Public | BindingFlags.Static);
        }
    }
}
