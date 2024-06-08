// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.AsyncExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using static AsyncTestExpressionExecutionContext;

public class When_using_async_expression_execution_context
{
    private static Exception ShouldHaveBeenReplacedException => new XunitException("should have been replaced by next strategy");

    [Fact]
    public async Task Should_apply_all_custom_strategies_and_return_expected_result()
    {
        var context = new AsyncTestExpressionExecutionContext();

        var result = await context.ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
    }

    [Fact]
    public async Task Should_apply_remote_expression_preparation_decorator()
    {
        var customExpression1 = new ConstantExpression("exp1");
        var customExpression2 = new ConstantExpression("exp2");

        var callCounter = new int[3];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With(x =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step1_PreparedRemoteExpression);
                return customExpression1;
            })
            .With(x =>
            {
                callCounter[1]++;
                x.ShouldBeSameAs(customExpression1);
                return customExpression2;
            })
            .With(x =>
            {
                callCounter[2]++;
                x.ShouldBeSameAs(customExpression2);
                return Step1_PreparedRemoteExpression;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_replace_expression_transformation_decorator()
    {
        var callCounter = new int[1];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With(new Func<Expression, System.Linq.Expressions.Expression>(x => throw ShouldHaveBeenReplacedException))
            .With(new Func<Expression, System.Linq.Expressions.Expression>(x => throw ShouldHaveBeenReplacedException))
            .With((Expression x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step1_PreparedRemoteExpression);
                return Step2_InitialTransformedSystemExpression;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex2_TransformRemoteExpression);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_apply_system_expression_preparation_decorator()
    {
        var customExpression1 = System.Linq.Expressions.Expression.Constant("local-exp1");
        var customExpression2 = System.Linq.Expressions.Expression.Constant("local-exp2");
        var customExpression3 = System.Linq.Expressions.Expression.Constant("local-exp3");
        var customExpression4 = System.Linq.Expressions.Expression.Constant("local-exp4");

        var callCounter = new int[5];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With((System.Linq.Expressions.Expression x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step3_PreparedSystemExpression);
                return customExpression1;
            })
            .With((System.Linq.Expressions.Expression x) =>
            {
                callCounter[1]++;
                x.ShouldBeSameAs(customExpression1);
                return customExpression2;
            })
            .With((System.Linq.Expressions.Expression x) =>
            {
                callCounter[2]++;
                x.ShouldBeSameAs(customExpression2);
                return Step3_PreparedSystemExpression;
            })
            .With((System.Linq.Expressions.Expression x, CancellationToken ct) =>
            {
                callCounter[3]++;
                x.ShouldBeSameAs(Step4_PreparedForAsyncExpression);
                return customExpression4;
            })
            .With((System.Linq.Expressions.Expression x, CancellationToken ct) =>
            {
                callCounter[4]++;
                x.ShouldBeSameAs(customExpression4);
                return Step4_PreparedForAsyncExpression;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_replace_expression_execution_decorator()
    {
        var callCounter = new int[1];

        var context = new AsyncTestExpressionExecutionContext();

        var result = context
            .With(new Func<System.Linq.Expressions.Expression, object>(_ => throw ShouldHaveBeenReplacedException))
            .With(new Func<System.Linq.Expressions.Expression, object>(_ => throw ShouldHaveBeenReplacedException))
            .With((System.Linq.Expressions.Expression x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step3_PreparedSystemExpression);
                return Step5_ExecutionResult;
            })
            .Execute();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: [StepIndex4_PreparedForAsyncExpression, StepIndex5_ExecuteExpression]);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_replace_async_expression_execution_decorator()
    {
        var callCounter = new int[1];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With(new Func<System.Linq.Expressions.Expression, CancellationToken, ValueTask<object>>((_, _) => throw ShouldHaveBeenReplacedException))
            .With(new Func<System.Linq.Expressions.Expression, CancellationToken, ValueTask<object>>((_, _) => throw ShouldHaveBeenReplacedException))
            .With((System.Linq.Expressions.Expression x, CancellationToken ct) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step4_PreparedForAsyncExpression);
                return new(Step5_ExecutionResult);
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex5_ExecuteExpression);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_apply_raw_result_processing_decorator()
    {
        var customResult1 = "result1";
        var customResult2 = "result2";

        var callCounter = new int[3];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With((object x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step6_ProcessedResult);
                return customResult1;
            })
            .With((object x) =>
            {
                callCounter[1]++;
                x.ShouldBeSameAs(customResult1);
                return customResult2;
            })
            .With((object x) =>
            {
                callCounter[2]++;
                x.ShouldBeSameAs(customResult2);
                return Step6_ProcessedResult;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_replace_result_to_dynamic_object_projection_decorator()
    {
        var callCounter = new int[1];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With(new Func<object, DynamicObject>(x => throw ShouldHaveBeenReplacedException))
            .With(new Func<object, DynamicObject>(x => throw ShouldHaveBeenReplacedException))
            .With((object x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step6_ProcessedResult);
                return Step7_ConvertedResult;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex7_ConvertResult);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public async Task Should_apply_dynamic_object_result_processing_decorator()
    {
        var customResult1 = new DynamicObject("result1");
        var customResult2 = new DynamicObject("result2");

        var callCounter = new int[3];

        var context = new AsyncTestExpressionExecutionContext();

        var result = await context
            .With((DynamicObject x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step8_FinalResult);
                return customResult1;
            })
            .With((DynamicObject x) =>
            {
                callCounter[1]++;
                x.ShouldBeSameAs(customResult1);
                return customResult2;
            })
            .With((DynamicObject x) =>
            {
                callCounter[2]++;
                x.ShouldBeSameAs(customResult2);
                return Step8_FinalResult;
            })
            .ExecuteAsync();

        result.ShouldBeSameAs(Step8_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }
}