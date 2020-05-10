// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class GenericExtensions
    {
        /// <summary>
        /// Returns null if either the value is null or the value matches the predicate. the original value otherwise.
        /// </summary>
        internal static T? NullIf<T>(this T? value, Func<T, bool> predicate)
            where T : struct
            => value.HasValue && predicate(value.Value) ? default : value;

        /// <summary>
        /// Returns null if the predicate is matched.
        /// </summary>
        internal static T NullIf<T>(this T value, Func<T, bool> predicate)
            where T : class
            => predicate(value) ? default : value;

        /// <summary>
        /// Returns the value retrieved from the value provider if the source value is null, otherwise the source value is returned.
        /// </summary>
        internal static T IfNull<T>(this T value, Func<T> valueProvider)
            where T : class
            => value is null ? valueProvider() : value;

        /// <summary>
        /// Returns the default value if the source value is null, otherwise the source value is returned.
        /// </summary>
        internal static T IfNull<T>(this T value, T defaultValue = default)
            where T : class
            => value is null ? defaultValue : value;
    }
}
