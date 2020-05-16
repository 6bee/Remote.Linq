// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeSystem;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class Helper
    {
        [return: NotNullIfNotNull("type")]
        public static TypeInfo? AsTypeInfo(this Type? type) => type is null ? null : new TypeInfo(type, false, false);

        [return: NotNullIfNotNull("method")]
        public static MethodInfo? AsMethodInfo(this System.Reflection.MethodInfo? method) => method is null ? null : new MethodInfo(method);

        /// <summary>
        /// Returns null if either the value is null or the value matches the predicate. The original value is returned otherwise.
        /// </summary>
        internal static T? NullIf<T>(this T? value, Func<T, bool> predicate)
            where T : struct
            => value.HasValue && predicate(value.Value) ? default : value;

        internal static Type GetElementTypeOrThrow(this Type collectionType)
            => TypeHelper.GetElementType(collectionType) ?? throw new RemoteLinqException($"Failed to get element type of {collectionType}.");

        internal static string QuoteValue(this object? value, string nullValue = "null")
            => string.Format(
                "{1}{0}{1}",
                value ?? nullValue,
                value is string ? @"""" : value is char ? "'" : null);
    }
}
