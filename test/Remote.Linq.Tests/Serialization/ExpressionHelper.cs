// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using System.Security;
using Xunit;

public static class ExpressionHelper
{
    [SecuritySafeCritical]
    public static void ShouldEqualRemoteExpression<T>(this T expression1, T expression2)
        where T : Remote.Linq.Expressions.Expression
        => ObjectStringsShouldBeEqual(expression1, expression2);

    /// <summary>
    /// Best effort comparison using Expression.ToString().
    /// </summary>
    [SecuritySafeCritical]
    public static void ShouldEqualExpression<T>(this T expression1, T expression2)
        where T : System.Linq.Expressions.Expression
        => ObjectStringsShouldBeEqual(expression1, expression2);

    private static void ObjectStringsShouldBeEqual(object o1, object o2)
    {
        var s1 = o1.ToString();
        var s2 = o2.ToString();
        Assert.Equal(s1, s2);
    }
}