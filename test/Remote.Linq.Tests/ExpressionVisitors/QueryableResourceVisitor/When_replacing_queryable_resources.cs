// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests.ExpressionVisitors.QueryableResourceVisitor;

using Aqua.Dynamic;
using Remote.Linq.DynamicQuery;
using Remote.Linq.ExpressionVisitors;
using System.Linq.Expressions;
using RemoteLinq = Remote.Linq.Expressions;

public class When_replacing_queryable_resources
{
    private class ProbeDto
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    private class StubQueryProvider : IRemoteQueryProvider
    {
        public System.Linq.Expressions.Expression Expression => throw new NotSupportedException();

        public Type ElementType => typeof(ProbeDto);

        public IQueryable CreateQuery(Expression source)
            => throw new NotSupportedException();

        public IQueryable<TElement> CreateQuery<TElement>(Expression source)
            => throw new NotSupportedException();

        public object Execute(Expression expression)
            => throw new NotSupportedException();

        public TResult Execute<TResult>(Expression expression)
            => throw new NotSupportedException();
    }

    [Fact]
    public void Should_replace_queryable_constant_by_resource_descriptor()
    {
        var queryable = new RemoteQueryable(typeof(ProbeDto), new StubQueryProvider());
        var constant = new RemoteLinq.ConstantExpression(queryable);

        var rewritten = constant.ReplaceQueryableByResourceDescriptors();

        rewritten.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var descriptor = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<QueryableResourceDescriptor>();
        descriptor.Type.ToType().ShouldBe(typeof(ProbeDto));
    }

    [Fact]
    public void Should_leave_enumerable_query_constant_unchanged()
    {
        var queryable = new[] { 1 }.AsQueryable();
        var constant = new RemoteLinq.ConstantExpression(queryable);

        var rewritten = constant.ReplaceQueryableByResourceDescriptors();

        rewritten.ShouldBeSameAs(constant);
        rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeSameAs(queryable);
    }

    [Fact]
    public void Should_replace_queryable_properties_inside_constant_query_argument()
    {
        var queryable = new RemoteQueryable(typeof(ProbeDto), new StubQueryProvider());
        var argument = new ConstantQueryArgument(new DynamicObject(new[]
        {
            ("Queryable", (object?)queryable),
            ("Count", (object?)42),
        }));
        var constant = new RemoteLinq.ConstantExpression(argument, typeof(ConstantQueryArgument));

        var rewritten = constant.ReplaceQueryableByResourceDescriptors();

        rewritten.ShouldNotBeSameAs(constant);
        var newArgument = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<ConstantQueryArgument>();
        var properties = newArgument.Value.Properties.ShouldNotBeNull();
        properties["Queryable"]
            .ShouldBeOfType<QueryableResourceDescriptor>()
            .Type.ToType().ShouldBe(typeof(ProbeDto));
        properties["Count"].ShouldBe(42);
    }

    [Fact]
    public void Should_copy_constant_query_argument_even_when_no_property_changes()
    {
        var argument = new ConstantQueryArgument(new DynamicObject(new[]
        {
            ("Count", (object?)42),
            ("Name", (object?)"probe"),
        }));
        var constant = new RemoteLinq.ConstantExpression(argument, typeof(ConstantQueryArgument));

        var rewritten = constant.ReplaceQueryableByResourceDescriptors();

        rewritten.ShouldNotBeSameAs(constant);
        var copy = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<ConstantQueryArgument>();
        copy.ShouldNotBeSameAs(argument);
        var properties = copy.Value.Properties.ShouldNotBeNull();
        properties["Count"].ShouldBe(42);
        properties["Name"].ShouldBe("probe");
    }

    [Fact]
    public void Should_replace_cancellation_token_by_substitution_value()
    {
        var constant = new RemoteLinq.ConstantExpression(CancellationToken.None, typeof(CancellationToken));

        var rewritten = constant.ReplaceQueryableByResourceDescriptors();

        rewritten.ShouldNotBeSameAs(constant);
        rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Type.ToType().ShouldBe(typeof(CancellationToken));
        var substitution = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<SubstitutionValue>();
        substitution.Type.ToType().ShouldBe(typeof(CancellationToken));
    }

    [Fact]
    public void Should_replace_resource_descriptor_using_queryable_provider()
    {
        var descriptor = new QueryableResourceDescriptor(typeof(ProbeDto));
        var constant = new RemoteLinq.ConstantExpression(descriptor);

        Type? requestedType = null;
        RemoteQueryable? returned = null;
        Func<Type, RemoteQueryable> provider = type =>
        {
            requestedType = type;
            returned = new RemoteQueryable(type, new StubQueryProvider());
            return returned;
        };

        var rewritten = constant.ReplaceResourceDescriptorsByQueryable(provider);

        requestedType.ShouldBe(typeof(ProbeDto));
        var value = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value;
        value.ShouldBeSameAs(returned);
        value.ShouldBeOfType<RemoteQueryable>().ElementType.ShouldBe(typeof(ProbeDto));
    }

    [Fact]
    public void Should_replace_nested_resource_descriptor_inside_constant_query_argument()
    {
        var descriptor = new QueryableResourceDescriptor(typeof(ProbeDto));
        var argument = new ConstantQueryArgument(new DynamicObject(new[]
        {
            ("Source", (object?)descriptor),
            ("Count", (object?)7),
        }));
        var constant = new RemoteLinq.ConstantExpression(argument, typeof(ConstantQueryArgument));

        Func<Type, RemoteQueryable> provider = type => new RemoteQueryable(type, new StubQueryProvider());
        var rewritten = constant.ReplaceResourceDescriptorsByQueryable(provider);

        var newArgument = rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<ConstantQueryArgument>();
        var properties = newArgument.Value.Properties.ShouldNotBeNull();
        properties["Source"]
            .ShouldBeOfType<RemoteQueryable>()
            .ElementType.ShouldBe(typeof(ProbeDto));
        properties["Count"].ShouldBe(7);
    }

    [Fact]
    public void Should_replace_cancellation_substitution_by_none()
    {
        var substitution = new SubstitutionValue(typeof(CancellationToken));
        var constant = new RemoteLinq.ConstantExpression(substitution, typeof(CancellationToken));

        Func<Type, RemoteQueryable> provider = _ => throw new NotSupportedException("Provider must not be called.");
        var rewritten = constant.ReplaceResourceDescriptorsByQueryable(provider);

        rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Type.ToType().ShouldBe(typeof(CancellationToken));
        rewritten.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBe(CancellationToken.None);
    }

    [Fact]
    public void Should_leave_unsupported_substitution_and_unrelated_constants_unchanged()
    {
        Func<Type, RemoteQueryable> provider = _ => new RemoteQueryable(typeof(ProbeDto), new StubQueryProvider());

        var substitutionConstant = new RemoteLinq.ConstantExpression(new SubstitutionValue(typeof(Guid)), typeof(Guid));
        var scalarConstant = new RemoteLinq.ConstantExpression("hello");

        substitutionConstant.ReplaceResourceDescriptorsByQueryable(provider).ShouldBeSameAs(substitutionConstant);
        scalarConstant.ReplaceResourceDescriptorsByQueryable(provider).ShouldBeSameAs(scalarConstant);
    }
}
