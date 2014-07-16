// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remote.Linq
{
    public static class ExpressionExtensions
    {
        public static IEnumerable<DynamicObject> Execute(this Remote.Linq.Expressions.Expression expression, Func<Type, System.Linq.IQueryable> queryableProvider)
        {
            var queryableExpression = expression.ReplaceResourceDescriptorsByQueryable(queryableProvider);

            var linqExpression = queryableExpression.ToLinqExpression();
            var lambdaExpression = linqExpression as System.Linq.Expressions.LambdaExpression;
            if (lambdaExpression == null)
            {
                lambdaExpression = System.Linq.Expressions.Expression.Lambda(linqExpression);
            }
            var result = lambdaExpression.Compile().DynamicInvoke();
            var enumerable = (IEnumerable<DynamicObject>)result;
            var list = enumerable.ToList();
            return list;
        }
    }
}
