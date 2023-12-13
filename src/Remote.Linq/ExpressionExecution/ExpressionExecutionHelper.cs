// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ExpressionExecution
{
    using System.ComponentModel;
    using System.Security;
    using SystemLinq = System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExecutionHelper
    {
        [SecuritySafeCritical]
        public static object? CompileAndInvokeExpression(this SystemLinq.Expression expression)
        {
            if (expression is SystemLinq.ConstantExpression constant)
            {
                return constant.Value;
            }

            var lambdaExpression =
                (expression as SystemLinq.LambdaExpression) ??
                SystemLinq.Expression.Lambda(expression);
            return lambdaExpression.Compile().DynamicInvokeAndUnwrap();
        }
    }
}