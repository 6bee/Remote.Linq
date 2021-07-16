// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Diagnostics.CodeAnalysis;

    internal static partial class MethodInfos
    {
        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        private sealed class TDelegate
        {
            private TDelegate()
            {
            }
        }

        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        internal sealed class TElement
        {
            private TElement()
            {
            }
        }

        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        internal sealed class TSource
        {
            private TSource()
            {
            }
        }

        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        internal sealed class TResult
        {
            private TResult()
            {
            }
        }

        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        private sealed class TKey
        {
            private TKey()
            {
            }
        }

        /// <summary>
        /// Type definition used in generic type filters.
        /// </summary>
        [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
        private sealed class TAccumulate
        {
            private TAccumulate()
            {
            }
        }
    }
}