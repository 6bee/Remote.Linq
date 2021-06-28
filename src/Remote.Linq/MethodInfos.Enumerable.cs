// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    static partial class MethodInfos
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

            internal static readonly MethodInfo Cast = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.Cast),
                new[] { typeof(TResult) },
                typeof(IEnumerable));

            internal static readonly MethodInfo OfType = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.OfType),
                new[] { typeof(TResult) },
                typeof(IEnumerable));

            internal static readonly MethodInfo ToArray = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.ToArray),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo ToList = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.ToList),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo Contains = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.Contains),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(TSource));

            internal static readonly MethodInfo Single = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.Single),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo SingleWithPredicate = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.Single),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(Func<TSource, bool>));

            internal static readonly MethodInfo SingleOrDefault = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.SingleOrDefault),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>));

            internal static readonly MethodInfo SingleOrDefaultWithPredicate = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.SingleOrDefault),
                new[] { typeof(TSource) },
                typeof(IEnumerable<TSource>),
                typeof(Func<TSource, bool>));
        }
    }
}