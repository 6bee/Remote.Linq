// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Remote.Linq.DynamicQuery;
    using Shouldly;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;

    public class When_translating_expression_back_and_forth : ExpressionTranslatorTestBase
    {
        private class A
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public bool Flag { get; set; }

            public int? Count { get; set; }
        }

        [Fact]
        public void Should_preserve_type_in_const()
        {
            BackAndForth(Expression.Constant(true, typeof(bool))).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_nullable_type_in_const()
        {
            BackAndForth(Expression.Constant(true, typeof(bool?))).ShouldMatch();
        }

        [Fact]
        [SuppressMessage("Minor Code Smell", "S1125:Boolean literals should not be redundant", Justification = "Intentional test setup")]
        public void Should_preserve_bool_in_lambda()
        {
            BackAndForth<Expression<Func<A, bool?>>>(a => a.Flag == true).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_nullable_bool_in_lambda()
        {
            BackAndForth<Expression<Func<A, bool?>>>(a => a.Flag == (bool?)true).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_nullable_bool_conversions_comparison_in_lambda()
        {
            BackAndForth<Expression<Func<A, bool?>>>(a => (bool?)a.Name.EndsWith("x") == (bool?)true).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_nullable_bool_const_comparison_in_lambda()
        {
            Expression<Func<A, bool?>> expr =
                p => (bool?)p.Name.EndsWith("x");

            var lambda = (LambdaExpression)expr;

            var predicate =
                Expression.Lambda<Func<A, bool>>(
                    Expression.Equal(
                        lambda.Body,
                        Expression.Constant(true, typeof(bool?))),
                    lambda.Parameters);

            BackAndForth(predicate).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_Modulo()
        {
            BackAndForth<Expression<Func<A, long>>>(a => a.Id % 2).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_bitwise_exclusive_or()
        {
            BackAndForth<Expression<Func<A, double>>>(a => a.Count.Value ^ 3).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_null_coalescing()
        {
            BackAndForth<Expression<Func<A, int>>>(a => a.Count ?? -1).ShouldMatch();
        }

        [Fact]
        public void Should_preserve_variable()
        {
            double e = Math.E;

            Expression<Func<double>> expression = () => e;

            BackAndForth(expression)
                .RoundTrip
                .Body.ShouldBeAssignableTo<MemberExpression>()
                .Expression.ShouldBeOfType<NewExpression>()
                .With(exp => exp.Type.ShouldBe(typeof(VariableQueryArgument<double>)))
                .Arguments.Single().ShouldBeOfType<ConstantExpression>()
                .Value.ShouldBe(Math.E);
        }

        [Fact]
        public void Should_preserve_func_expression()
        {
            var func = new Func<int, int>(x => x);
            var newFunc = BackAndForth<Expression<Func<int, int>>>(x => func(x)).RoundTrip.Compile();
            var r = newFunc(9);
            r.ShouldBe(9);
        }

        [Fact]
#if NETCOREAPP
        [SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions", Justification = "Cannot assert")]
#endif // NETCOREAPP
        public void Should_preserve_action_expression()
        {
            var action = new Action<int>(x => { });
            var newAction = BackAndForth<Expression<Action<int>>>(x => action(x)).RoundTrip.Compile();
            newAction(9);

#if NETFRAMEWORK
            newAction.Target
                .ShouldBeOfType<System.Runtime.CompilerServices.Closure>()
                .Constants.Single().ShouldBeOfType<Action<int>>().Method.ShouldBeSameAs(action.Method);
#endif // NETFRAMEWORK
        }

        [Fact]
        public void Should_preserve_method_expression()
        {
            BackAndForth<Expression<Func<int, int>>>(x => Method(x)).ShouldMatch();
        }

        private int Method(int i)
        {
            return i;
        }
    }
}