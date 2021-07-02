// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using System.Reflection;

    partial class MethodInfos
    {
        internal static class Expression
        {
            /// <summary>
            /// Type definition used in generic type filters.
            /// </summary>
            [SuppressMessage("Major Bug", "S3453:Classes should not have only \"private\" constructors", Justification = "For reflection only")]
            internal sealed class TDelegate
            {
                private TDelegate()
                {
                }
            }

            internal static readonly MethodInfo Lambda = GetMethod(
                typeof(System.Linq.Expressions.Expression),
                nameof(System.Linq.Expressions.Expression.Lambda),
                new[] { typeof(TDelegate) },
                typeof(System.Linq.Expressions.Expression),
                typeof(ParameterExpression[]));
        }
    }
}