// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionTranslator.Context;

using Remote.Linq.ExpressionExecution;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SystemLinq = System.Linq.Expressions;

public class When_translating_expression
{
    public IEnumerable<int> Enumerable => System.Linq.Enumerable.Range(0, 99);

    public IQueryable<int> Queryable => Enumerable.AsQueryable();

    [Fact]
    public void Should_project_grouping_elements()
    {
        var grouping =
            from x in Enumerable
            group x by x % 2 into g
            select g;

        var result = RoundtripValue(grouping);

        var array = result.ShouldBeOfType<IGrouping<int, int>[]>();
        array.Length.ShouldBe(2);

        array[0].ShouldBeOfType<Grouping<int, int>>().Key.ShouldBe(0);
        array[1].ShouldBeOfType<Grouping<int, int>>().Key.ShouldBe(1);
    }

    private static SystemLinq.Expression RoundtripExpression<TExpression>(TExpression expression)
        where TExpression : SystemLinq.Expression
    {
        var remotelinq = expression.ToRemoteLinqExpression();
        var systemlinq = remotelinq.ToLinqExpression();
        return systemlinq;
    }

    private static object RoundtripValue<T>(T value)
    {
        var expression = SystemLinq.Expression.Constant(value, typeof(T));
        var result = RoundtripExpression(expression);
        return result.CompileAndInvokeExpression();
    }
}