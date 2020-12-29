// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.ComponentModel;
    using System.Security;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionHelper
    {
        [SecuritySafeCritical]
        public static object? CompileAndInvokeExpression(this System.Linq.Expressions.Expression expression)
        {
            if (expression is System.Linq.Expressions.ConstantExpression constant)
            {
                return constant.Value;
            }

            var lambdaExpression =
                (expression as System.Linq.Expressions.LambdaExpression) ??
                System.Linq.Expressions.Expression.Lambda(expression);
            return lambdaExpression.Compile().DynamicInvokeAndUnwrap();
        }
    }
}
