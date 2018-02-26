// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq.Expressions;

    public static class ExpressionEvaluator
    {
        public static bool CanBeEvaluated(Expression expression)
        {
#if !NETSTANDARD1_3 && !NETSTANDARD1_6
            if ((expression as MemberExpression)?.Member.DeclaringType == typeof(EF))
            {
                return false;
            }
#endif

            return true;
        }

        internal static Func<Expression, bool> And(this Func<Expression, bool> func1, Func<Expression, bool> func2)
        {
            if (func1 == null) return func2;
            if (func2 == null) return func1;
            return expression => func1(expression) && func2(expression);
        }
    }
}
