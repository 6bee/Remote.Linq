// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Linq;
    using RemoteExpression = Remote.Linq.Expressions.Expression;
    using SystemExpression = System.Linq.Expressions.Expression;

    internal static class ExpressionHelper
    {
        internal static void CheckExpressionResultType<TResult>(SystemExpression expression)
        {
            var expressionType = expression.CheckNotNull(nameof(expression)).Type;
            if (typeof(TResult).IsAssignableFrom(expressionType))
            {
                return;
            }

            if (typeof(IRemoteResource).IsAssignableFrom(expressionType))
            {
                return;
            }

            if (expressionType.Implements(typeof(IQueryable<>), out var typeArgs) &&
                typeArgs.Length == 1 &&
                typeof(TResult).IsAssignableFrom(typeArgs[0]))
            {
                return;
            }

            throw new ArgumentException("The specified expression is not assignable to the result type.", nameof(expression));
        }

        internal static RemoteExpression TranslateExpression(SystemExpression expression, ITypeInfoProvider? typeInfoProvider, Func<SystemExpression, bool>? canBeEvaluatedLocally)
        {
            var slinq1 = expression.CheckNotNull(nameof(expression)).SimplifyIncorporationOfRemoteQueryables();
            var rlinq1 = slinq1.ToRemoteLinqExpression(typeInfoProvider, canBeEvaluatedLocally);
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors(typeInfoProvider);
            var rlinq3 = rlinq2.ReplaceGenericQueryArgumentsByNonGenericArguments();
            return rlinq3;
        }
    }
}
