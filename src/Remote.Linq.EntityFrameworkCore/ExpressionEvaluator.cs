// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public static class ExpressionEvaluator
{
    public static bool CanBeEvaluated(Expression expression)
    {
        if ((expression as MemberExpression)?.Member.DeclaringType == typeof(EF))
        {
            return false;
        }

        return true;
    }
}