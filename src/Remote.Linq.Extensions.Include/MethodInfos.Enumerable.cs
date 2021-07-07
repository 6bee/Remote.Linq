// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
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

            internal static readonly MethodInfo SelectMethodInfo = GetMethod(
                typeof(System.Linq.Enumerable),
                nameof(System.Linq.Enumerable.Select),
                new[] { typeof(TSource), typeof(TResult) },
                typeof(IEnumerable<TSource>),
                typeof(Func<TSource, TResult>));
        }
    }
}