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
    public static readonly Expression Step0_InitialRemoteExpression = new ConstantExpression("step0");
    public static readonly Expression Step1_PreparedRemoteExpression = new ConstantExpression("step1");
    public static readonly System.Linq.Expressions.Expression Step2_InitialTransformedSystemExpression = System.Linq.Expressions.Expression.Constant("step2");
    public static readonly System.Linq.Expressions.Expression Step3_PreparedSystemExpression = System.Linq.Expressions.Expression.Constant("step3");
    public static readonly object Step4_ExecutionResult = "step4";
    public static readonly object Step5_ProcessedResult = "step5";
    public static readonly DynamicObject Step6_ConvertedResult = new("step6");
    public static readonly DynamicObject Step7_FinalResult = new("step7");

    public static readonly int StepIndex2_InitialTransformedSystemExpression = 1;
    public static readonly int StepIndex4_ExecutionResult = 3;
    public static readonly int StepIndex6_ConvertedResult = 5;

    private readonly int[] _callCounters = new int[7];

    public TestExpressionExecutionContext(DefaultExpressionExecutor parent = null, Expression expression = null)
        : base(parent ?? new DefaultExpressionExecutor(null), expression ?? Step0_InitialRemoteExpression)
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
        expression.ShouldBeSameAs(Step0_InitialRemoteExpression);
        return Step1_PreparedRemoteExpression;
    }

    protected override System.Linq.Expressions.Expression Transform(Expression expression)
    {
        _callCounters[1]++;
        expression.ShouldBeSameAs(Step1_PreparedRemoteExpression);
        return Step2_InitialTransformedSystemExpression;
    }

    protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
    {
        _callCounters[2]++;
        expression.ShouldBeSameAs(Step2_InitialTransformedSystemExpression);
        return Step3_PreparedSystemExpression;
    }

    protected override object Execute(System.Linq.Expressions.Expression expression)
    {
        _callCounters[3]++;
        expression.ShouldBeSameAs(Step3_PreparedSystemExpression);
        return Step4_ExecutionResult;
    }

    protected override object ProcessResult(object queryResult)
    {
        _callCounters[4]++;
        queryResult.ShouldBeSameAs(Step4_ExecutionResult);
        return Step5_ProcessedResult;
    }

    protected override DynamicObject ConvertResult(object queryResult)
    {
        _callCounters[5]++;
        queryResult.ShouldBeSameAs(Step5_ProcessedResult);
        return Step6_ConvertedResult;
    }

    protected override DynamicObject ProcessConvertedResult(DynamicObject queryResult)
    {
        _callCounters[6]++;
        queryResult.ShouldBeSameAs(Step6_ConvertedResult);
        return Step7_FinalResult;
    }
}