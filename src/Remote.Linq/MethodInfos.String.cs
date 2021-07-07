// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeExtensions;
    using System.Reflection;

    partial class MethodInfos
    {
        internal static class String
        {
            internal static readonly MethodInfo StartsWith = typeof(string).GetMethodEx(nameof(string.StartsWith), typeof(string));
            internal static readonly MethodInfo EndsWith = typeof(string).GetMethodEx(nameof(string.EndsWith), typeof(string));
            internal static readonly MethodInfo Contains = typeof(string).GetMethodEx(nameof(string.Contains), typeof(string));
        }
    }
}