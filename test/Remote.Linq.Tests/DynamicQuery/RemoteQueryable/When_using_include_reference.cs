// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.RemoteQueryable
{
    using Remote.Linq;
    using Aqua.Dynamic;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using Xunit.Fluent;

    public class When_using_include_reference
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

        public When_using_include_reference()
        {
            Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
            {
                expression = exp;
                return new DynamicObject[0];
            };

            var queryable = RemoteQueryable.Create<Child>(dataProvider);

            queryable
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
            ((MethodCallExpression)expression).Method.Name.ShouldBe("Include");
        }

        [Fact]
        public void Expression_should_have_two_arguments()
        {
            ((MethodCallExpression)expression).Arguments.Count.ShouldBe(2);
        }

        [Fact]
        public void First_argument_should_be_constant_expression_with_resource_descriptor()
        {
            var arg = ((MethodCallExpression)expression).Arguments[0];
            
            arg.NodeType.ShouldBe(ExpressionType.Constant);
            ((ConstantExpression)arg).Value.ShouldBeOfType<QueryableResourceDescriptor>();
            ((QueryableResourceDescriptor)((ConstantExpression)arg).Value).Type.Type.ShouldBe(typeof(Child));
        }

        [Fact]
        public void Second_argument_should_be_constant_expression_with_navigation_property_name()
        {
            var arg = ((MethodCallExpression)expression).Arguments[1];

            arg.NodeType.ShouldBe(ExpressionType.Constant);
            ((ConstantExpression)arg).Value.ShouldBe("Parent");
        }
     }
}