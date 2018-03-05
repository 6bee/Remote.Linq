// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Expressions.ExpressionExecutionContext
{
    using Remote.Linq.Expressions;
    using System.Linq;
    using System.Collections.Generic;
    using Aqua.Dynamic;
    using Shouldly;

    public class TestExpressionExecutionContext : ExpressionExecutionContext
    {
        public readonly static Expression Step0_Expression = new ConstantExpression("step0");
        public readonly static Expression Step1_Expression = new ConstantExpression("step1");
        public readonly static System.Linq.Expressions.Expression Step2_Expression = System.Linq.Expressions.Expression.Constant("step2");
        public readonly static System.Linq.Expressions.Expression Step3_Expression = System.Linq.Expressions.Expression.Constant("step3");
        public readonly static object Step4_Result = "step4";
        public readonly static object Step5_Result = "step5";
        public readonly static IEnumerable<DynamicObject> Step6_Result = new[] { new DynamicObject("step6") };
        public readonly static IEnumerable<DynamicObject> Step7_Result = new[] { new DynamicObject("step7") };

        private readonly int[] _callCounters = new int[7];

        public TestExpressionExecutionContext(ExpressionExecutor parent, Expression expression) 
            : base(parent, expression)
        {
        }

        public void AssertAllMethodsInvokedExacltyOnce(params int[] skip)
        {
            var expectedCounters = (
                from c in _callCounters.Select((x, i) => new { Count = x, Index = i })
                where !skip.Contains(c.Index)
                select c
                ).ToArray();

            expectedCounters.Count().ShouldBe(_callCounters.Count() - skip.Length);
            expectedCounters.ShouldAllBe(x => x.Count == 1, 
                $"processor of {nameof(TestExpressionExecutionContext)} should be called since they are decorated rather then replaced by custom strategy");

            var unexpectedCounters = (
                from c in _callCounters.Select((x, i) => new { Count = x, Index = i })
                where skip.Contains(c.Index)
                select c
                ).ToArray();

            unexpectedCounters.Count().ShouldBe(skip.Length);
            unexpectedCounters.ShouldAllBe(x => x.Count == 0, 
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

        protected override IEnumerable<DynamicObject> ConvertResult(object queryResult)
        {
            _callCounters[5]++;
            queryResult.ShouldBeSameAs(Step5_Result);
            return Step6_Result;
        }

        protected override IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
        {
            _callCounters[6]++;
            queryResult.ShouldBeSameAs(Step6_Result);
            return Step7_Result;
        }
    }
}
