// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Remote.Linq;
    using Shouldly;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Xunit;

    public class When_subquery_expression_use_same_parameter_name
    {
        class A
        {
            public int Value { get; set; }
        }

        class B
        {
            public int Value { get; set; }
        }

        [Fact]
        public void Result_should_be_equal_for_translated_expression()
        {
            var dates1 = new[]
            {
                new DateTime(1999,1,31),
                new DateTime(1999,1,15),
                new DateTime(2000,1,31),
                new DateTime(2000,1,15),
            }.AsQueryable();

            var dates2 = new[]
            {
                new DateTime(1995,1,27),
                new DateTime(1996,2,28),
                new DateTime(1997,3,29),
                new DateTime(1998,4,30),
                new DateTime(1999,5,31),
            }.AsQueryable();
            
            Expression<Func<DateTime, bool>> subfilter = x => x.Day == 31;
            Expression<Func<DateTime, bool>> outerfilter = x => dates2.Where(subfilter).Where(d => d.Year == x.Year).Any();
            var result1 = dates1.Where(outerfilter).ToArray();

            var remoteLinqExpression = outerfilter.ToRemoteLinqExpression();
            var systemlinqExpression = remoteLinqExpression.ToLinqExpression<DateTime, bool>();
            var result2 = dates1.Where(systemlinqExpression).ToArray();

            result1.ShouldMatch(result2);
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name()
        {
            var list1 = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            var list2 = new[]
            {
                new B { Value = 1 },
                new B { Value = 2 },
                new B { Value = 3 },
                new B { Value = 4 },
            }.AsQueryable();

            Expression<Func<B, bool>> subfilter = x => x.Value % 2 == 0;
            Expression<Func<A, bool>> outerfilter = x => list2.Where(subfilter).Where(d => d.Value == x.Value).Any();
            var result1 = list1.Where(outerfilter).ToArray();

            var remoteLinqExpression = outerfilter.ToRemoteLinqExpression();
            var systemlinqExpression = remoteLinqExpression.ToLinqExpression<A, bool>();
            var result2 = list1.Where(systemlinqExpression).ToArray();

            result1.ShouldMatch(result2);
        }

        [Fact]
        public void Parameter_expression_should_be_resolved_by_instance_rather_then_by_name2()
        {
            var list1 = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            var list2 = new[]
            {
                new A { Value = 1 },
                new A { Value = 2 },
                new A { Value = 3 },
                new A { Value = 4 },
            }.AsQueryable();

            Expression<Func<A, bool>> subfilter = x => x.Value % 2 == 0;
            Expression<Func<A, bool>> outerfilter = x => list2.Where(subfilter).Where(d => d.Value == x.Value).Any();
            var result1 = list1.Where(outerfilter).ToArray();

            var remoteLinqExpression = outerfilter.ToRemoteLinqExpression();
            var systemlinqExpression = remoteLinqExpression.ToLinqExpression<A, bool>();
            var result2 = list1.Where(systemlinqExpression).ToArray();

            result1.ShouldMatch(result2);
        }
    }
}