// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Microsoft.EntityFrameworkCore;
using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using Remote.Linq.Include;
using System.Linq.Expressions;

public class When_rewriting_ef_core_expressions
{
    [Fact]
    public void Should_replace_remote_include_method_with_entity_framework_include_method()
    {
        var source = new List<Parent>().AsQueryable();
        var expression = Expression.Call(
            IncludeQueryableExtensions.StringIncludeMethodInfo.MakeGenericMethod(typeof(Parent)),
            source.Expression,
            Expression.Constant(nameof(Parent.Children)));

        var result = (MethodCallExpression)expression.ReplaceIncludeQueryMethods();

        result.Method.DeclaringType.ShouldBe(typeof(EntityFrameworkQueryableExtensions));
        result.Method.Name.ShouldBe(nameof(EntityFrameworkQueryableExtensions.Include));
    }

    [Fact]
    public void Should_leave_non_include_calls_unchanged_and_wrap_queryable_constants()
    {
        var value = Expression.Constant(1);
        value.ReplaceIncludeQueryMethods().ShouldBeSameAs(value);

        var queryable = new List<LookupItem>().AsQueryable();
        var wrapped = Expression.Constant(queryable).WrapQueryableInClosure();

        wrapped.NodeType.ShouldBe(ExpressionType.MemberAccess);
        wrapped.Type.ShouldBe(typeof(IQueryable<LookupItem>));
    }

    private sealed class Parent
    {
        public IEnumerable<LookupItem> Children { get; set; } = [];
    }
}
