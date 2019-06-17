// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    partial class Expression
    {
        [Obsolete("This method will be removed in future versions. Use actual constructor instead.", true)]
        public static MethodCallExpression MethodCall(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
            => Call(insatnce, methodName, declaringType, bindingFlags, genericArguments, parameterTypes, arguments);

        [Obsolete("This method will be removed in future versions. Use actual constructor instead.", false)]
        public static MethodCallExpression Call(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
        {
            return new MethodCallExpression(insatnce, methodName, declaringType, bindingFlags, genericArguments, parameterTypes, arguments);
        }
    }
}
