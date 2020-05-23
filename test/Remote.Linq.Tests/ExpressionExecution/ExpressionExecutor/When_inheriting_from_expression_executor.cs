// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionExecution.ExpressionExecutor
{
    using Aqua.Dynamic;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class When_inheriting_from_expression_executor
    {
        [Fact]
        public void Should_apply_all_custom_strategies_and_return_expected_result()
        {
            var executor = new TestExpressionExecutor();

            executor
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce();
        }

        [Fact]
        public void Should_execute_test_expression_using_default_executor()
        {
            TestExpressionExecutor.Step0_Expression
                .Execute(null)
                .ShouldHaveSingleItem()
                .Values
                .ShouldHaveSingleItem()
                .ShouldBe("step0");
        }

        [Fact]
        public void Should_apply_remote_expression_preparation_executor()
        {
            var customExpression1 = new ConstantExpression("exp1");
            var customExpression2 = new ConstantExpression("exp2");

            var callCounter = new int[3];

            var executor = new TestExpressionExecutor();

            executor
                .With(x =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step1_Expression);
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
                    return TestExpressionExecutor.Step1_Expression;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce();
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_replace_expression_transformation_executor()
        {
            var callCounter = new int[1];

            var executor = new TestExpressionExecutor();

            executor
                .With(new Func<Expression, System.Linq.Expressions.Expression>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With(new Func<Expression, System.Linq.Expressions.Expression>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With((Expression x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step1_Expression);
                    return TestExpressionExecutor.Step2_Expression;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce(skip: 1);
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_apply_system_expression_preparation_executor()
        {
            var customExpression1 = System.Linq.Expressions.Expression.Constant("exp1");
            var customExpression2 = System.Linq.Expressions.Expression.Constant("exp2");

            var callCounter = new int[3];

            var executor = new TestExpressionExecutor();

            executor
                .With((System.Linq.Expressions.Expression x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step3_Expression);
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
                    return TestExpressionExecutor.Step3_Expression;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce();
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_replace_expression_execution_executor()
        {
            var callCounter = new int[1];

            var executor = new TestExpressionExecutor();

            executor
                .With(new Func<System.Linq.Expressions.Expression, object>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With(new Func<System.Linq.Expressions.Expression, object>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With((System.Linq.Expressions.Expression x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step3_Expression);
                    return TestExpressionExecutor.Step4_Result;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce(skip: 3);
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_apply_raw_result_processing_executor()
        {
            var customResult1 = "result1";
            var customResult2 = "result2";

            var callCounter = new int[3];

            var executor = new TestExpressionExecutor();

            executor
                .With((object x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step5_Result);
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
                    return TestExpressionExecutor.Step5_Result;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce();
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_replace_result_to_dynamic_object_projection_executor()
        {
            var callCounter = new int[1];

            var executor = new TestExpressionExecutor();

            executor
                .With(new Func<object, IEnumerable<DynamicObject>>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With(new Func<object, IEnumerable<DynamicObject>>(x =>
                {
                    throw new Exception("should have been replaced by next strategy");
                }))
                .With((object x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step5_Result);
                    return TestExpressionExecutor.Step6_Result;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce(skip: 5);
            callCounter.ShouldAllBe(x => x == 1);
        }

        [Fact]
        public void Should_apply_dynamic_object_result_processing_executor()
        {
            var customResult1 = new[] { new DynamicObject("result1") };
            var customResult2 = new[] { new DynamicObject("result2") };

            var callCounter = new int[3];

            var executor = new TestExpressionExecutor();

            executor
                .With((IEnumerable<DynamicObject> x) =>
                {
                    callCounter[0]++;
                    x.ShouldBeSameAs(TestExpressionExecutor.Step7_Result);
                    return customResult1;
                })
                .With((IEnumerable<DynamicObject> x) =>
                {
                    callCounter[1]++;
                    x.ShouldBeSameAs(customResult1);
                    return customResult2;
                })
                .With((IEnumerable<DynamicObject> x) =>
                {
                    callCounter[2]++;
                    x.ShouldBeSameAs(customResult2);
                    return TestExpressionExecutor.Step7_Result;
                })
                .Execute(TestExpressionExecutor.Step0_Expression)
                .ShouldBeSameAs(TestExpressionExecutor.Step7_Result);

            executor.AssertAllMethodsInvokedExacltyOnce();
            callCounter.ShouldAllBe(x => x == 1);
        }
    }
}
