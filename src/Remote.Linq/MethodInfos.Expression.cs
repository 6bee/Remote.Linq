// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq;

using Aqua.TypeExtensions;
using System.Linq.Expressions;
using System.Reflection;

partial class MethodInfos
{
    internal static class Expression
    {
        internal static readonly MethodInfo Lambda = typeof(System.Linq.Expressions.Expression).GetMethodEx(
            nameof(System.Linq.Expressions.Expression.Lambda),
            [typeof(TDelegate)],
            typeof(System.Linq.Expressions.Expression),
            typeof(ParameterExpression[]));
    }
}