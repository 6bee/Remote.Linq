// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Include
{
    using Aqua.Dynamic;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Remote.Linq.Include;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using QueryableResourceDescriptor = Remote.Linq.DynamicQuery.QueryableResourceDescriptor;

    public abstract class When_using_include
    {
        public class With_include_path_string : When_using_include
        {
            public With_include_path_string()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(DataProvider);

                _ = queryable
                    .Include("Children.Parent.Children.Parent")
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Children.Parent.Children.Parent" };
        }

        public class With_include_path : When_using_include
        {
            public With_include_path()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(DataProvider);

                _ = queryable
                    .Include(x => x.Children.Select(y => y.Parent.Children.Select(z => z.Parent)))
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Children.Parent.Children.Parent" };
        }

        public class With_include_reference_collection : When_using_include
        {
            public With_include_reference_collection()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(DataProvider);

                _ = queryable
                    .Include(x => x.Children)
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Children" };
        }

        public class With_include_on_subtype : When_using_include
        {
            public With_include_on_subtype()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(DataProvider);

                _ = queryable
                    .SelectMany(x => x.Children)
                    .Include(x => x.Parent)
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Parent" };
        }

        public class With_include_reference : When_using_include
        {
            public With_include_reference()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(DataProvider);

                _ = queryable
                    .Include(x => x.Parent)
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Parent" };

            protected override Type QueryResourceType => typeof(Child);
        }

        public class With_multiple_include : When_using_include
        {
            public With_multiple_include()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(DataProvider);

                _ = queryable
                    .Include(x => x.Parent).ThenInclude(x => x.Children)
                    .Include(x => x.Siblings).ThenInclude(x => x.Siblings)
                    .Include(x => x.Siblings).ThenInclude(x => x.Parent)
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Parent.Children", "Siblings.Siblings", "Siblings.Parent" };

            protected override Type QueryResourceType => typeof(Child);
        }

        public class With_then_include : When_using_include
        {
            public With_then_include()
            {
                var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(DataProvider);

                _ = queryable
                    .Include(x => x.Children).ThenInclude(x => x.Parent).ThenInclude(x => x.Children).ThenInclude(x => x.Parent)
                    .ToList();
            }

            protected override string[] ExpectedIncludePaths => new[] { "Children.Parent.Children.Parent" };
        }

        private class Child
        {
            public Parent Parent { get; set; }

            public IEnumerable<Child> Siblings { get; set; }
        }

        private class Parent
        {
            public IEnumerable<Child> Children { get; set; }
        }

        public When_using_include()
        {
            DataProvider = exp =>
            {
                Expression = exp.ReplaceIncludeQueryMethodsByStringInclude();
                return new DynamicObject(Array.Empty<object>());
            };
        }

        protected virtual Func<Expression, DynamicObject> DataProvider { get; }

        protected virtual Expression Expression { get; private set; }

        protected abstract string[] ExpectedIncludePaths { get; }

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
                .Type.ToType().ShouldBe(QueryResourceType);
        }

        [Fact]
        public void Second_argument_should_be_constant_expression_with_navigation_property_name()
        {
            var expectedIncludePaths = ExpectedIncludePaths.Reverse().ToArray();

            void AssertIncludePath(MethodCallExpression expression, int index = 0)
            {
                if (expression.Arguments[0] is MethodCallExpression baseExpression && baseExpression.Method?.Name == "Include")
                {
                    AssertIncludePath(baseExpression, index + 1);
                }

                var arg = expression.Arguments[1];
                arg.NodeType.ShouldBe(ExpressionType.Constant);
                arg.ShouldBeOfType<ConstantExpression>().Value.ShouldBe(expectedIncludePaths[index]);
            }

            AssertIncludePath((MethodCallExpression)Expression);
        }
    }
}
