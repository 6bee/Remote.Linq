// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.RemoteQueryable
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class When_using_include_on_subtype
    {
        class Child
        {
            public Parent Parent { get; set; }
        }

        class Parent
        {
            public IEnumerable<Child> Children { get; set; }
        }

        Expression expression;

        public When_using_include_on_subtype()
        {
            Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
            {
                expression = exp;
                return new DynamicObject[0];
            };

            var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(dataProvider);

            queryable
                .SelectMany(x => x.Children)
                .Include(x => x.Parent)
                .ToList();
        }

        [Fact]
        public void Expression_should_be_set()
        {
            expression.ShouldNotBeNull();
        }

        [Fact]
        public void Expression_should_be_method_call_expression()
        {
            expression.ShouldBeOfType<MethodCallExpression>();
        }

        [Fact]
        public void Expression_should_be_include_method_call()
        {
            expression.ShouldBeOfType<MethodCallExpression>().Method.Name.ShouldBe("Include");
        }

        [Fact]
        public void Expression_should_have_two_arguments()
        {
            expression.ShouldBeOfType<MethodCallExpression>().Arguments.Count.ShouldBe(2);
        }

        [Fact]
        public void First_argument_should_be_method_call_expression_returning_a_queryable()
        {
            var arg = expression.ShouldBeOfType<MethodCallExpression>().Arguments[0];
            
            arg.NodeType.ShouldBe(ExpressionType.Call);
            arg.ShouldBeOfType<MethodCallExpression>()
                .Method.ShouldBeOfType<MethodInfo>()
                .Method.ReturnType.ShouldBe(typeof(IQueryable<Child>));
        }

        [Fact]
        public void Second_argument_should_be_constant_expression_with_navigation_property_name()
        {
            var arg = expression.ShouldBeOfType<MethodCallExpression>().Arguments[1];

            arg.NodeType.ShouldBe(ExpressionType.Constant);
            arg.ShouldBeOfType<ConstantExpression>()
                .Value.ShouldBe("Parent");
        }
     }
}