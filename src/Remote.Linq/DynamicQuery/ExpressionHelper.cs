// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeExtensions;
    using System;
    using System.Linq;
    using SystemExpression = System.Linq.Expressions.Expression;

    public static class ExpressionHelper
    {
        /// <summary>
        /// Checks whether the give <see cref="SystemExpression"/> is assignable
        /// to the given <typeparamref name="TResult"/> type in any form,
        /// throws an <see cref="ArgumentException"/> otherwise.
        /// </summary>
        public static void CheckExpressionResultType<TResult>(SystemExpression expression)
        {
            var expressionType = expression.CheckNotNull(nameof(expression)).Type;
            if (typeof(TResult).IsAssignableFrom(expressionType))
            {
                return;
            }

            if (typeof(IRemoteLinqQueryable).IsAssignableFrom(expressionType))
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
    }
}