// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Include;

using Remote.Linq.Include;

public class When_using_include_with_local_queryables
{
    [Fact]
    public void Should_return_local_queryables_without_modifying_their_expression()
    {
        var source = new[] { new Parent() }.AsQueryable();

        source.Include("Children").ShouldBeSameAs(source);
        source.Include(parent => parent.Children).ToArray().ShouldBe(source);
        source.Include(parent => parent.Children).ThenInclude(child => child.Parent).ToArray().ShouldBe(source);
    }

    [Fact]
    public void Should_reject_missing_include_arguments()
    {
        Should.Throw<ArgumentNullException>(() => ((IQueryable<Parent>)null).Include("Children"));
        Should.Throw<ArgumentException>(() => new[] { new Parent() }.AsQueryable().Include(string.Empty));
        Should.Throw<ArgumentNullException>(() => new[] { new Parent() }.AsQueryable().Include((System.Linq.Expressions.Expression<Func<Parent, IEnumerable<Child>>>)null));
    }

    private sealed class Parent
    {
        public IEnumerable<Child> Children { get; set; } = [];
    }

    private sealed class Child
    {
        public Parent Parent { get; set; } = new();
    }
}
