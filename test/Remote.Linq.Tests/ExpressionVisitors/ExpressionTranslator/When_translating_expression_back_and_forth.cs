// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Remote.Linq;
    using Shouldly;
    using System;
    using System.Linq.Expressions;
    using Xunit;

    public class When_translating_expression_back_and_forth
    {
        class A
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
                .Item2
                .Body.ShouldBeAssignableTo<MemberExpression>()
                .Expression.ShouldBeOfType<ConstantExpression>()
                .Value.ShouldBeOfType<VariableQueryArgument<double>>()
                .Value.ShouldBe(Math.E);
        }


        private static Tuple<T, T> BackAndForth<T>(T expression) where T : Expression
        {
            //Console.WriteLine($">original: {expression}");

            var remoteExpression = expression.ToRemoteLinqExpression();
            //Console.WriteLine($">remote:   {remoteExpression}");

            var linqExpression = remoteExpression.ToLinqExpression();
            //Console.WriteLine($">linq:     {linqExpression}");

            return Tuple.Create(expression, (T)linqExpression);
        }
    }
}