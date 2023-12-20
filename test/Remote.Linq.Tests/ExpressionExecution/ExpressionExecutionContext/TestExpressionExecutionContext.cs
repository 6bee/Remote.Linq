// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.ExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Shouldly;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

[SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
public class TestExpressionExecutionContext : DefaultExpressionExecutionContext
{
    public static readonly Expression Step0_Expression = new ConstantExpression("step0");
    public static readonly Expression Step1_Expression = new ConstantExpression("step1");
    public static readonly System.Linq.Expressions.Expression Step2_Expression = System.Linq.Expressions.Expression.Constant("step2");
    public static readonly System.Linq.Expressions.Expression Step3_Expression = System.Linq.Expressions.Expression.Constant("step3");
    public static readonly object Step4_Result = "step4";
    public static readonly object Step5_Result = "step5";
    public static readonly DynamicObject Step6_Result = new DynamicObject("step6");
    public static readonly DynamicObject Step7_Result = new DynamicObject("step7");

    private readonly int[] _callCounters = new int[7];

    public TestExpressionExecutionContext(DefaultExpressionExecutor parent, Expression expression)
        : base(parent, expression)
    {
    }

    public void AssertAllMethodsInvokedExacltyOnce(params int[] skip)
    {
        var expectedCounters = (
            from c in _callCounters.Select((x, i) => new { Count = x, Index = i })
            where !skip.Contains(c.Index)
            select c)
            .ToArray();

        expectedCounters.Length.ShouldBe(_callCounters.Length - skip.Length);
        expectedCounters.ShouldAllBe(
            x => x.Count == 1,
            $"processor of {nameof(TestExpressionExecutionContext)} should be called since they are decorated rather then replaced by custom strategy");

        var unexpectedCounters = (
            from c in _callCounters.Select((x, i) => new { Count = x, Index = i })
            where skip.Contains(c.Index)
            select c)
            .ToArray();

        unexpectedCounters.Length.ShouldBe(skip.Length);
        unexpectedCounters.ShouldAllBe(
            x => x.Count == 0,
            $"processor of {nameof(TestExpressionExecutionContext)} should be called since they are replaced rather then decorated with custom strategy");
    }

    protected override Expression Prepare(Expression expression)
    {
        _callCounters[0]++;
        expression.ShouldBeSameAs(Step0_Expression);
        return Step1_Expression;
    }

    protected override System.Linq.Expressions.Expression Transform(Expression expression)
    {
        _callCounters[1]++;
        expression.ShouldBeSameAs(Step1_Expression);
        return Step2_Expression;
    }

    protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
    {
        _callCounters[2]++;
        expression.ShouldBeSameAs(Step2_Expression);
        return Step3_Expression;
    }

    protected override object Execute(System.Linq.Expressions.Expression expression)
    {
        _callCounters[3]++;
        expression.ShouldBeSameAs(Step3_Expression);
        return Step4_Result;
    }

    protected override object ProcessResult(object queryResult)
    {
        _callCounters[4]++;
        queryResult.ShouldBeSameAs(Step4_Result);
        return Step5_Result;
    }

    protected override DynamicObject ConvertResult(object queryResult)
    {
        _callCounters[5]++;
        queryResult.ShouldBeSameAs(Step5_Result);
        return Step6_Result;
    }

    protected override DynamicObject ProcessResult(DynamicObject queryResult)
    {
        _callCounters[6]++;
        queryResult.ShouldBeSameAs(Step6_Result);
        return Step7_Result;
    }
}