// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization
{
    using System.Security;
    using Xunit;

    public static class ExpressionHelper
    {
        [SecuritySafeCritical]
        public static void EqualsRemoteExpression<T>(this T expression1, T expression2) where T : Remote.Linq.Expressions.Expression
        {
            var expression1String = expression1.ToString();
            var expression2String = expression2.ToString();

            Assert.Equal(expression1String, expression2String);
        }

        /// <summary>
        /// Best effort comparison using Expression.ToString().
        /// </summary>
        [SecuritySafeCritical]
        public static void EqualsExpression<T>(this T expression1, T expression2) where T : System.Linq.Expressions.Expression
        {
            var expression1String = expression1.ToString();
            var expression2String = expression2.ToString();

            Assert.Equal(expression1String, expression2String);
        }
    }
}
