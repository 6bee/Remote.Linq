// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.ExpressionVisitors.ExpressionTranslator
{
    using Shouldly;
    using System;
    using System.Linq.Expressions;
    using Xunit;

    public class When_translating_expression_with_local_variables
    {
        private class A
        {
            public short Property { get; set; }
        }

        [Fact]
        public void Should_resolve_short_argument_to_variable()
        {
            short argument = 1;

            Expression<Func<A, bool>> expr = x => x.Property == argument;

            var remoteExpr = expr.ToRemoteLinqExpression();

            var stringRepresentation = remoteExpr.ToString();
            stringRepresentation.ShouldBe("x => (Convert(x.Property, Int32) Equal Convert(new Remote.Linq.DynamicQuery.VariableQueryArgument`1[System.Int16](1).Value, Int32))");
        }

        [Fact]
        public void Should_resolve_int_argument_to_variable()
        {
            int parameter = 1;

            Expression<Func<A, bool>> expr = x => x.Property == parameter;

            var remoteExpr = expr.ToRemoteLinqExpression();

            var stringRepresentation = remoteExpr.ToString();
            stringRepresentation.ShouldBe("x => (Convert(x.Property, Int32) Equal new Remote.Linq.DynamicQuery.VariableQueryArgument`1[System.Int32](1).Value)");
        }
    }
}