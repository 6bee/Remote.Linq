﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.ExpressionExecutionContext;

using Aqua.Dynamic;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;
using Shouldly;
using System;
using Xunit;
using Xunit.Sdk;
using static TestExpressionExecutionContext;

public class When_using_expression_execution_context
{
    private static Exception ShouldHaveBeenReplacedException => new XunitException("should have been replaced by next strategy");

    [Fact]
    public void Should_apply_all_custom_strategies_and_return_expected_result()
    {
        var context = new TestExpressionExecutionContext();

        context
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
    }

    [Fact]
    public void Should_apply_remote_expression_preparation_decorator()
    {
        var customExpression1 = new ConstantExpression("exp1");
        var customExpression2 = new ConstantExpression("exp2");

        var callCounter = new int[3];

        var context = new TestExpressionExecutionContext();

        context
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
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_replace_expression_transformation_decorator()
    {
        var callCounter = new int[1];

        var context = new TestExpressionExecutionContext();

        context
            .With(new Func<Expression, System.Linq.Expressions.Expression>(x => throw ShouldHaveBeenReplacedException))
            .With(new Func<Expression, System.Linq.Expressions.Expression>(x => throw ShouldHaveBeenReplacedException))
            .With((Expression x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step1_PreparedRemoteExpression);
                return Step2_InitialTransformedSystemExpression;
            })
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex2_InitialTransformedSystemExpression);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_apply_system_expression_preparation_decorator()
    {
        var customExpression1 = System.Linq.Expressions.Expression.Constant("exp1");
        var customExpression2 = System.Linq.Expressions.Expression.Constant("exp2");

        var callCounter = new int[3];

        var context = new TestExpressionExecutionContext();

        context
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
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_replace_expression_execution_decorator()
    {
        var callCounter = new int[1];

        var context = new TestExpressionExecutionContext();

        context
            .With(new Func<System.Linq.Expressions.Expression, object>(x => throw ShouldHaveBeenReplacedException))
            .With(new Func<System.Linq.Expressions.Expression, object>(x => throw ShouldHaveBeenReplacedException))
            .With((System.Linq.Expressions.Expression x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step3_PreparedSystemExpression);
                return Step4_ExecutionResult;
            })
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex4_ExecutionResult);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_apply_raw_result_processing_decorator()
    {
        var customResult1 = "result1";
        var customResult2 = "result2";

        var callCounter = new int[3];

        var context = new TestExpressionExecutionContext();

        context
            .With((object x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step5_ProcessedResult);
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
                return Step5_ProcessedResult;
            })
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_replace_result_to_dynamic_object_projection_decorator()
    {
        var callCounter = new int[1];

        var context = new TestExpressionExecutionContext();

        context
            .With(new Func<object, DynamicObject>(x => throw ShouldHaveBeenReplacedException))
            .With(new Func<object, DynamicObject>(x => throw ShouldHaveBeenReplacedException))
            .With((object x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step5_ProcessedResult);
                return Step6_ConvertedResult;
            })
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce(skip: StepIndex6_ConvertedResult);
        callCounter.ShouldAllBe(x => x == 1);
    }

    [Fact]
    public void Should_apply_dynamic_object_result_processing_decorator()
    {
        var customResult1 = new DynamicObject("result1");
        var customResult2 = new DynamicObject("result2");

        var callCounter = new int[3];

        var context = new TestExpressionExecutionContext();

        context
            .With((DynamicObject x) =>
            {
                callCounter[0]++;
                x.ShouldBeSameAs(Step7_FinalResult);
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
                return Step7_FinalResult;
            })
            .Execute()
            .ShouldBeSameAs(Step7_FinalResult);

        context.AssertAllMethodsInvokedExacltyOnce();
        callCounter.ShouldAllBe(x => x == 1);
    }
}