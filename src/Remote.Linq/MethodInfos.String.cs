// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Reflection;

    internal static partial class MethodInfos
    {
        internal static class String
        {
            internal static readonly MethodInfo StartsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) }) !;
            internal static readonly MethodInfo EndsWith = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) }) !;
            internal static readonly MethodInfo Contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }) !;
        }
    }
}