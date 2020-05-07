// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Expressions
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System.Linq;
    using Xunit;
    using Expression = System.Linq.Expressions.Expression;

    public class When_using_remote_linq_as_linq_provider
    {
        private class TestModel
        {
        }

        private IQueryable<TestModel> Items => new[] { new TestModel() }.AsQueryable();

        private IQueryable<TestModel> RemoteItems => RemoteQueryable.Factory.CreateQueryable<TestModel>(exp => exp.Execute(t => Items)); // .Where(x => x != null);

        [Fact]
        public void Should_execute_with_normal_enumeration()
        {
            RemoteItems.Single().ShouldBeOfType<TestModel>();
        }

        [Fact]
        public void Should_execute_as_untyped_queryable()
        {
            var queryable = (IQueryable)RemoteItems;
            var count = 0;
            foreach (object item in queryable)
            {
                count++;
                item.ShouldBeOfType<TestModel>();
            }

            count.ShouldBe(1);
        }

        [Fact]
        public void Should_support_method_call_expression()
        {
            var src = RemoteItems;
            var result = (int)src.Provider.Execute(Expression.Call(typeof(Queryable), nameof(Queryable.Count), new[] { src.ElementType }, new[] { src.Expression }));
            result.ShouldBe(1);
        }
    }
}
