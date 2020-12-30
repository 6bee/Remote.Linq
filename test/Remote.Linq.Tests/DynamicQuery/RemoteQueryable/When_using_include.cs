// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.RemoteQueryable
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public abstract class When_using_include
    {
        public class With_include_path : When_using_include
        {
            private Expression _expression;

            public With_include_path()
            {
                Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
                {
                    _expression = exp;
                    return new DynamicObject[0];
                };

                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(dataProvider);

                _ = queryable
                    .Include(x => x.Children.Select(y => y.Parent.Children.Select(z => z.Parent)))
                    .ToList();
            }

            protected override Expression Expression => _expression;

            protected override string ExpectedIncludePath => "Children.Parent.Children.Parent";
        }

        public class With_include_reference_collection : When_using_include
        {
            private Expression _expression;

            public With_include_reference_collection()
            {
                Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
                {
                    _expression = exp;
                    return new DynamicObject[0];
                };

                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(dataProvider);

                queryable
                    .Include(x => x.Children)
                    .ToList();
            }

            protected override Expression Expression => _expression;

            protected override string ExpectedIncludePath => "Children";
        }

        public class With_include_on_subtype : When_using_include
        {
            private Expression _expression;

            public With_include_on_subtype()
            {
                Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
                {
                    _expression = exp;
                    return new DynamicObject[0];
                };

                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(dataProvider);

                queryable
                    .SelectMany(x => x.Children)
                    .Include(x => x.Parent)
                    .ToList();
            }

            protected override Expression Expression => _expression;

            protected override string ExpectedIncludePath => "Parent";
        }

        public class With_include_reference : When_using_include
        {
            private Expression _expression;

            public With_include_reference()
            {
                Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
                {
                    _expression = exp;
                    return new DynamicObject[0];
                };

                var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(dataProvider);

                queryable
                    .Include(x => x.Parent)
                    .ToList();
            }

            protected override Expression Expression => _expression;

            protected override string ExpectedIncludePath => "Parent";

            protected override Type QueryResourceType => typeof(Child);
        }

        public class With_then_include : When_using_include
        {
            private Expression _expression;

            public With_then_include()
            {
                Func<Expression, IEnumerable<DynamicObject>> dataProvider = exp =>
                {
                    _expression = exp;
                    return new DynamicObject[0];
                };

                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(dataProvider);

                _ = queryable
                    .Include(x => x.Children).ThenInclude(x => x.Parent).ThenInclude(x => x.Children).ThenInclude(x => x.Parent)
                    .ToList();
            }

            protected override Expression Expression => _expression;

            protected override string ExpectedIncludePath => "Children.Parent.Children.Parent";
        }

        private class Child
        {
            public Parent Parent { get; set; }
        }

        private class Parent
        {
            public IEnumerable<Child> Children { get; set; }
        }

        protected abstract Expression Expression { get; }

        protected abstract string ExpectedIncludePath { get; }

        protected virtual Type QueryResourceType => typeof(Parent);

        [Fact]
        public void Expression_should_be_set()
        {
            Expression.ShouldNotBeNull();
        }

        [Fact]
        public void Expression_should_be_method_call_expression()
        {
            Expression.ShouldBeOfType<MethodCallExpression>();
        }

        [Fact]
        public void Expression_should_be_include_method_call()
        {
            Expression.ShouldBeOfType<MethodCallExpression>().Method.Name.ShouldBe("Include");
        }

        [Fact]
        public void Expression_should_have_two_arguments()
        {
            Expression.ShouldBeOfType<MethodCallExpression>().Arguments.Count.ShouldBe(2);
        }

        [Fact]
        public void First_argument_should_be_constant_expression_with_resource_descriptor()
        {
            var resourceExpression = ((MethodCallExpression)Expression).Arguments[0];

            while (resourceExpression.NodeType == ExpressionType.Call)
            {
                resourceExpression = ((MethodCallExpression)resourceExpression).Arguments[0];
            }

            resourceExpression.NodeType.ShouldBe(ExpressionType.Constant);

            resourceExpression.ShouldBeOfType<ConstantExpression>()
                .Value.ShouldBeOfType<QueryableResourceDescriptor>()
                .Type.Type.ShouldBe(QueryResourceType);
        }

        [Fact]
        public void Second_argument_should_be_constant_expression_with_navigation_property_name()
        {
            var arg = ((MethodCallExpression)Expression).Arguments[1];
            arg.NodeType.ShouldBe(ExpressionType.Constant);
            arg.ShouldBeOfType<ConstantExpression>().Value.ShouldBe(ExpectedIncludePath);
        }
    }
}
