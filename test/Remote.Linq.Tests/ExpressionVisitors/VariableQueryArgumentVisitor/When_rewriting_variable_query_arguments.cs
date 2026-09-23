// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

#nullable enable
namespace Remote.Linq.Tests.ExpressionVisitors.VariableQueryArgumentVisitor;

using Remote.Linq.DynamicQuery;
using Remote.Linq.ExpressionVisitors;
using RemoteLinq = Remote.Linq.Expressions;

public class When_rewriting_variable_query_arguments
{
    private class ProbeDto
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void Should_replace_generic_scalar_argument_by_non_generic_argument()
    {
        var constant = new RemoteLinq.ConstantExpression(new VariableQueryArgument<int>(42));

        var rewritten = constant.ReplaceGenericQueryArgumentsByNonGenericArguments();

        rewritten.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var argument = rewritten.Value.ShouldBeOfType<VariableQueryArgument>();
        argument.Type.ToType().ShouldBe(typeof(int));
        argument.Value.ShouldBe(42);
    }

    [Fact]
    public void Should_replace_generic_collection_argument_by_non_generic_list_argument()
    {
        var constant = new RemoteLinq.ConstantExpression(new VariableQueryArgument<List<int>>([1, 2, 3]));

        var rewritten = constant.ReplaceGenericQueryArgumentsByNonGenericArguments();

        rewritten.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var argument = rewritten.Value.ShouldBeOfType<VariableQueryArgumentList>();
        argument.ElementType.ToType().ShouldBe(typeof(int));
        argument.Values.Count.ShouldBe(3);
        argument.Values.Cast<int>().ShouldBeSequenceEqual([1, 2, 3]);
    }

    [Fact]
    public void Should_replace_non_generic_scalar_argument_by_generic_argument()
    {
        var constant = new RemoteLinq.ConstantExpression(new VariableQueryArgument(42, typeof(int)));

        var rewritten = constant.ReplaceNonGenericQueryArgumentsByGenericArguments();

        rewritten.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var argument = rewritten.Value.ShouldBeOfType<VariableQueryArgument<int>>();
        argument.Value.ShouldBe(42);
    }

    [Fact]
    public void Should_replace_non_generic_list_argument_by_generic_collection_argument()
    {
        var constant = new RemoteLinq.ConstantExpression(new VariableQueryArgumentList(new int[] { 1, 2 }, typeof(int)));

        var rewritten = constant.ReplaceNonGenericQueryArgumentsByGenericArguments();

        rewritten.NodeType.ShouldBe(RemoteLinq.ExpressionType.Constant);
        var argument = rewritten.Value.ShouldBeOfType<VariableQueryArgument<List<int>>>();
        argument.Value.ShouldBeSequenceEqual([1, 2]);
    }

    [Fact]
    public void Should_rewrite_scalar_value_member_access()
    {
        var argument = new VariableQueryArgument<int>(42);
        var memberAccess = new RemoteLinq.MemberExpression(
            new RemoteLinq.ConstantExpression(argument),
            typeof(VariableQueryArgument<int>).GetProperty(nameof(VariableQueryArgument<>.Value))!);

        var toNonGeneric = memberAccess.ReplaceGenericQueryArgumentsByNonGenericArguments();
        toNonGeneric.Member.Name.ShouldBe(nameof(VariableQueryArgument.Value));
        toNonGeneric.Member.DeclaringType.ShouldNotBeNull().ToType().ShouldBe(typeof(VariableQueryArgument));
        toNonGeneric.Expression.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<VariableQueryArgument>();

        var restored = toNonGeneric.ReplaceNonGenericQueryArgumentsByGenericArguments();
        restored.Member.Name.ShouldBe(nameof(VariableQueryArgument<>.Value));
        restored.Member.DeclaringType.ShouldNotBeNull().ToType().ShouldBe(typeof(VariableQueryArgument<int>));
        restored.Expression.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<VariableQueryArgument<int>>();
    }

    [Fact]
    public void Should_rewrite_collection_values_member_access()
    {
        var argument = new VariableQueryArgument<List<int>>([1, 2]);
        var memberAccess = new RemoteLinq.MemberExpression(
            new RemoteLinq.ConstantExpression(argument),
            typeof(VariableQueryArgument<List<int>>).GetProperty(nameof(VariableQueryArgument<>.Value))!);

        var toNonGeneric = memberAccess.ReplaceGenericQueryArgumentsByNonGenericArguments();
        toNonGeneric.Member.Name.ShouldBe(nameof(VariableQueryArgumentList.Values));
        toNonGeneric.Member.DeclaringType.ShouldNotBeNull().ToType().ShouldBe(typeof(VariableQueryArgumentList));
        toNonGeneric.Expression.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<VariableQueryArgumentList>();

        var restored = toNonGeneric.ReplaceNonGenericQueryArgumentsByGenericArguments();
        restored.Member.Name.ShouldBe(nameof(VariableQueryArgument<>.Value));
        restored.Member.DeclaringType.ShouldNotBeNull().ToType().ShouldBe(typeof(VariableQueryArgument<List<int>>));
        restored.Expression.ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<VariableQueryArgument<List<int>>>();
    }

    [Fact]
    public void Should_leave_unrelated_constants_and_members_unchanged()
    {
        var constant = new RemoteLinq.ConstantExpression(42);
        var dto = new ProbeDto { Count = 5, Name = "probe" };
        var memberAccess = new RemoteLinq.MemberExpression(
            new RemoteLinq.ConstantExpression(dto),
            typeof(ProbeDto).GetProperty(nameof(ProbeDto.Name))!);

        constant.ReplaceGenericQueryArgumentsByNonGenericArguments().ShouldBeSameAs(constant);
        memberAccess.ReplaceGenericQueryArgumentsByNonGenericArguments().ShouldBeSameAs(memberAccess);

        constant.ReplaceNonGenericQueryArgumentsByGenericArguments().ShouldBeSameAs(constant);
        memberAccess.ReplaceNonGenericQueryArgumentsByGenericArguments().ShouldBeSameAs(memberAccess);
    }

    [Fact]
    public void Should_throw_for_unexpected_rewritten_argument_instance()
    {
        var memberAccess = new RemoteLinq.MemberExpression(
            new RemoteLinq.ConstantExpression("not a query argument"),
            typeof(VariableQueryArgument<int>).GetProperty(nameof(VariableQueryArgument<>.Value))!);

        var ex = Should.Throw<RemoteLinqException>(() => memberAccess.ReplaceGenericQueryArgumentsByNonGenericArguments());
        ex.Message.ShouldStartWith("Expected instance expression");
    }
}
