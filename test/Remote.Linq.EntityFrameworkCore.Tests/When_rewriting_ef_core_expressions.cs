// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests;

using Aqua.Dynamic;
using Aqua.TypeExtensions;
using Microsoft.EntityFrameworkCore;
using Remote.Linq.DynamicQuery;
using Remote.Linq.EntityFrameworkCore.DynamicQuery;
using Remote.Linq.EntityFrameworkCore.ExpressionVisitors;
using Remote.Linq.EntityFrameworkCore.Tests.Model;
using Remote.Linq.Include;
using System.Linq.Expressions;
using System.Reflection;

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

    [Fact]
    public void Should_map_lambda_include_and_then_include_methods_to_entity_framework_equivalents()
    {
        var source = new List<Parent>().AsQueryable();

        Expression<Func<Parent, IEnumerable<LookupItem>>> includeLambda = parent => parent.Children;
        var lambdaInclude = Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Parent), typeof(IEnumerable<LookupItem>)),
            source.Expression,
            Expression.Quote(includeLambda));
        var mappedLambdaInclude = (MethodCallExpression)lambdaInclude.ReplaceIncludeQueryMethods();
        mappedLambdaInclude.Method.DeclaringType.ShouldBe(typeof(EntityFrameworkQueryableExtensions));
        mappedLambdaInclude.Method.Name.ShouldBe(nameof(EntityFrameworkQueryableExtensions.Include));

        Expression<Func<Parent, LookupItem>> referenceInclude = parent => parent.Child;
        var referenceInnerInclude = Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Parent), typeof(LookupItem)),
            source.Expression,
            Expression.Quote(referenceInclude));
        Expression<Func<LookupItem, string>> referenceThenInclude = item => item.Key;
        var referenceThen = Expression.Call(
            IncludeQueryableExtensions.ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(typeof(Parent), typeof(LookupItem), typeof(string)),
            referenceInnerInclude,
            Expression.Quote(referenceThenInclude));
        var mappedReferenceThen = (MethodCallExpression)referenceThen.ReplaceIncludeQueryMethods();
        mappedReferenceThen.Method.DeclaringType.ShouldBe(typeof(EntityFrameworkQueryableExtensions));
        mappedReferenceThen.Method.Name.ShouldBe(nameof(EntityFrameworkQueryableExtensions.ThenInclude));

        var enumerableInnerInclude = Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Parent), typeof(IEnumerable<LookupItem>)),
            source.Expression,
            Expression.Quote(includeLambda));
        Expression<Func<LookupItem, string>> enumerableThenInclude = item => item.Value;
        var enumerableThen = Expression.Call(
            IncludeQueryableExtensions.ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(typeof(Parent), typeof(LookupItem), typeof(string)),
            enumerableInnerInclude,
            Expression.Quote(enumerableThenInclude));
        var mappedEnumerableThen = (MethodCallExpression)enumerableThen.ReplaceIncludeQueryMethods();
        mappedEnumerableThen.Method.DeclaringType.ShouldBe(typeof(EntityFrameworkQueryableExtensions));
        mappedEnumerableThen.Method.Name.ShouldBe(nameof(EntityFrameworkQueryableExtensions.ThenInclude));
    }

    [Fact]
    public void Should_leave_non_include_method_calls_unchanged()
    {
        var source = new List<LookupItem>().AsQueryable();
        var countMethod = typeof(Queryable)
            .GetMethodEx(nameof(Queryable.Count), [typeof(LookupItem)], [typeof(IQueryable<LookupItem>)])
            .MakeGenericMethod(typeof(LookupItem));
        var expression = Expression.Call(countMethod, source.Expression);

        expression.ReplaceIncludeQueryMethods().ShouldBeSameAs(expression);
    }

    [Fact]
    public void Should_not_wrap_non_queryable_constants()
    {
        var value = Expression.Constant(42);

        value.WrapQueryableInClosure().ShouldBeSameAs(value);
    }

    [Fact]
    public void Should_return_element_type_for_collection_expression_in_create_non_generic()
    {
        var provider = CreateProvider();
        var collectionExpression = Expression.Constant(new List<LookupItem>().AsQueryable());

        var queryable = RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(provider, collectionExpression);

        queryable.ResourceType.ShouldBe(typeof(LookupItem));
    }

    [Fact]
    public void Should_throw_for_scalar_expression_in_create_non_generic()
    {
        var provider = CreateProvider();
        var scalarExpression = new NullTypeExpression();

        var exception = Should.Throw<RemoteLinqException>(() => RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(provider, scalarExpression));

        exception.Message.ShouldStartWith("Cannot get element type based on expression's type");
    }

    [Fact]
    public void Should_reuse_constructor_cache_for_same_element_type()
    {
        var provider = CreateProvider();

        var first = RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(typeof(LookupItem), provider, null);
        var second = RemoteLinqEfCoreAsyncQueryable.CreateNonGeneric(typeof(LookupItem), provider, null);

        first.ShouldNotBeNull();
        second.ShouldNotBeNull();
        first.ResourceType.ShouldBe(typeof(LookupItem));
        second.ResourceType.ShouldBe(typeof(LookupItem));
    }

    private static IRemoteLinqEfCoreAsyncQueryProvider CreateProvider()
        => new RemoteLinqEfCoreAsyncQueryProvider<DynamicObject>(
            (_, _) => throw new NotSupportedException(),
            context: null,
            resultMapper: new AsyncDynamicResultMapper());

    private sealed class Parent
    {
        public LookupItem Child { get; set; }

        public IEnumerable<LookupItem> Children { get; set; } = [];
    }

    /// <summary>
    /// Expression that does not report a type. In Aqua 6.0.0-alpha-002 <c>TypeHelper.GetElementType</c> only returns
    /// <c>null</c> for a <c>null</c> input type, so this is the minimal non-collection-shaped expression that reaches
    /// the <c>RemoteLinqException</c> guard in <c>CreateNonGeneric</c>.
    /// </summary>
    private sealed class NullTypeExpression : Expression
    {
        public override ExpressionType NodeType => ExpressionType.Constant;

        public override Type Type => null;

        public override string ToString() => nameof(NullTypeExpression);
    }
}
