// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Expressions.ExpressionExecutor
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System.Collections.Generic;
    using Xunit;

    public class When_inheriting_from_expression_executor
    {
        class Exec : ExpressionExecutor
        {
            public readonly static Expression Step0_Expression = new ConstantExpression();
            public readonly static Expression Step1_Expression = new ConstantExpression();
            public readonly static System.Linq.Expressions.Expression Step2_Expression = System.Linq.Expressions.Expression.Constant(null, typeof(object));
            public readonly static System.Linq.Expressions.Expression Step3_Expression = System.Linq.Expressions.Expression.Constant(null, typeof(object));
            public readonly static object Step4_Result = new object();
            public readonly static object Step5_Result = new object();
            public readonly static IEnumerable<DynamicObject> Step6_Result = new DynamicObject[0];
            public readonly static IEnumerable<DynamicObject> Step7_Result = new DynamicObject[0];

            public Exec()
                : base(null)
            {
            }

            protected override Expression Prepare(Expression expression)
            {
                expression.ShouldBeSameAs(Step0_Expression);
                return Step1_Expression;
            }

            protected override System.Linq.Expressions.Expression Transform(Expression expression)
            {
                expression.ShouldBeSameAs(Step1_Expression);
                return Step2_Expression;
            }

            protected override System.Linq.Expressions.Expression Prepare(System.Linq.Expressions.Expression expression)
            {
                expression.ShouldBeSameAs(Step2_Expression);
                return Step3_Expression;
            }

            protected override object Execute(System.Linq.Expressions.Expression expression)
            {
                expression.ShouldBeSameAs(Step3_Expression);
                return Step4_Result;
            }

            protected override object ProcessResult(object queryResult)
            {
                queryResult.ShouldBeSameAs(Step4_Result);
                return Step5_Result;
            }

            protected override IEnumerable<DynamicObject> ConvertResult(object queryResult)
            {
                queryResult.ShouldBeSameAs(Step5_Result);
                return Step6_Result;
            }

            protected override IEnumerable<DynamicObject> ProcessResult(IEnumerable<DynamicObject> queryResult)
            {
                queryResult.ShouldBeSameAs(Step6_Result);
                return Step7_Result;
            }
        }

        [Fact]
        public void Should_apply_all_custom_strategies_and_return_expected_result()
        {
            new Exec().Execute(Exec.Step0_Expression).ShouldBeSameAs(Exec.Step7_Result);
        }
    }
}
