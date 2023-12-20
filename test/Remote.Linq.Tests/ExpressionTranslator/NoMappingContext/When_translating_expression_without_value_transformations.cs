// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionTranslator.NoMappingContext;

using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using SystemLinq = System.Linq.Expressions;

public class When_translating_expression_without_value_transformations
{
    private const BindingFlags PublicStatic = BindingFlags.Public | BindingFlags.Static;

    [Theory]
    [MemberData(nameof(GetSampleExpressions))]
    public void Should_translat_back_to_exact_same_expression(SystemLinq.Expression expression)
    {
        var rlinq = expression.ToRemoteLinqExpression(ExpressionTranslatorContext.NoMappingContext);
        var slinq = rlinq.ToLinqExpression(ExpressionTranslatorContext.NoMappingContext);

        var s0 = expression.ToString();
        var s1 = slinq.ToString();
        s1.ShouldBe(s0);
    }

    public static IEnumerable<object[]> GetSampleExpressions()
        => Expressions.Select(x => new object[] { x });

    public static IEnumerable<SystemLinq.Expression> Expressions
    {
        get
        {
            yield return SystemLinq.Expression.Constant(true);

            yield return SystemLinq.Expression.Call(
                typeof(When_translating_expression_without_value_transformations)
                    .GetMethod(nameof(GetSampleExpressions), PublicStatic));

            yield return SystemLinq.Expression.Call(
                SystemLinq.Expression.Constant(new When_translating_expression_without_value_transformations()),
                typeof(When_translating_expression_without_value_transformations)
                    .GetMethod(nameof(Should_translat_back_to_exact_same_expression)),
                SystemLinq.Expression.Constant(
                    SystemLinq.Expression.Constant(true)));

            var intParam = SystemLinq.Expression.Parameter(typeof(int));
            yield return SystemLinq.Expression.Lambda<Func<int, double>>(
                SystemLinq.Expression.MakeUnary(SystemLinq.ExpressionType.ConvertChecked, intParam, typeof(double)),
                intParam);
        }
    }
}