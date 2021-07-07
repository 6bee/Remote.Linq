// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using Aqua.TypeExtensions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    partial class MethodInfos
    {
        internal static class Enumerable
        {
            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
            private sealed class TSource
            {
                private TSource()
                {
                }
            }

            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
            private sealed class TResult
            {
                private TResult()
                {
                }
            }

            internal static readonly MethodInfo Cast = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.Cast),
                new[] { typeof(TResult) },
                typeof(IEnumerable));

            internal static readonly MethodInfo OfType = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.OfType),
                new[] { typeof(TResult) },
                typeof(IEnumerable));

            internal static readonly MethodInfo ToArray = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.ToArray),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo ToList = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.ToList),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo Contains = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.Contains),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(TSource));

            internal static readonly MethodInfo Single = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.Single),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo SingleWithPredicate = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.Single),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(Func<TSource, bool>));

            internal static readonly MethodInfo SingleOrDefault = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.SingleOrDefault),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo SingleOrDefaultWithPredicate = typeof(System.Linq.Enumerable).GetMethodEx(
                nameof(System.Linq.Enumerable.SingleOrDefault),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(Func<TSource, bool>));
        }
    }
}