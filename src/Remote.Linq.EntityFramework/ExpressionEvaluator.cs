// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFramework
{
    using Aqua.TypeExtensions;
    using System.Linq;
    using System.Linq.Expressions;

    public static class ExpressionEvaluator
    {
        public static bool CanBeEvaluated(Expression expression)
        {
            if ((expression as MethodCallExpression)?.Method.DeclaringType == typeof(System.Data.Entity.QueryableExtensions))
            {
                return false;
            }

            if (expression is ConstantExpression constant && constant.Type.Implements(typeof(IQueryable<>)))
            {
                return false;
            }

            return true;
        }
    }
}
