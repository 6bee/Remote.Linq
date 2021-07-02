// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Reflection;

    partial class MethodInfos
    {
        internal static class String
        {
            internal static readonly MethodInfo StartsWith = GetMethod(typeof(string), nameof(string.StartsWith), typeof(string));
            internal static readonly MethodInfo EndsWith = GetMethod(typeof(string), nameof(string.EndsWith), typeof(string));
            internal static readonly MethodInfo Contains = GetMethod(typeof(string), nameof(string.Contains), typeof(string));
        }
    }
}