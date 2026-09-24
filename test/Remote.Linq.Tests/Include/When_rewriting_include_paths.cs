// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Include;

using Aqua.Dynamic;
using Remote.Linq;
using Remote.Linq.Include;
using System.Reflection;
using QueryableResourceDescriptor = Remote.Linq.DynamicQuery.QueryableResourceDescriptor;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Tests the include rewriters in <see cref="SystemIncludeExpressionReWriter"/> and
/// <see cref="RemoteIncludeExpressionReWriter"/> through their public rewrite entry points.
/// </summary>
public class When_rewriting_include_paths
{
    private class Child
    {
        public Parent Parent { get; set; }

        public IEnumerable<Child> Siblings { get; set; }
    }

    private class Parent
    {
        public IEnumerable<Child> Children { get; set; }
    }

    public static IQueryable<T> CustomStringInclude<T>(IQueryable<T> source, string navigationPropertyPath)
        => source;

    private static readonly MethodInfo CustomStringIncludeDefinition =
        typeof(When_rewriting_include_paths).GetMethod(nameof(CustomStringInclude), BindingFlags.Static | BindingFlags.Public)!;

    [Fact]
    public void Should_convert_reference_then_include_to_string_include()
    {
        // Remote side: a reference navigation (Child.Parent) followed by a
        // reference-based ThenInclude (Parent.Children) must be collapsed into a
        // single string-include call carrying the full navigation path.
        var provider = CreateProvider();
        var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(provider.DataProvider);
        _ = queryable
            .Include(x => x.Parent)
            .ThenInclude(x => x.Children)
            .ToList();
        var remoteTree = provider.LastExpression.ShouldNotBeNull();

        var rewritten = remoteTree.ReplaceIncludeQueryMethodsByStringInclude();

        var call = rewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteStringIncludeCall(call, typeof(Child), "Parent.Children");

        // System side: the same chain built as a system expression tree must be
        // rewritten to the equivalent string-include call over the original
        // queryable constant.
        var systemQueryable = new[] { new Child() }.AsQueryable();
        SystemLinq.Expression<Func<Child, Parent>> referenceNav = x => x.Parent;
        SystemLinq.Expression<Func<Parent, IEnumerable<Child>>> collectionNav = x => x.Children;
        var includeCall = SystemLinq.Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Child), typeof(Parent)),
            SystemLinq.Expression.Constant(systemQueryable),
            SystemLinq.Expression.Quote(referenceNav));
        var thenIncludeCall = SystemLinq.Expression.Call(
            IncludeQueryableExtensions.ThenIncludeAfterReferenceMethodInfo.MakeGenericMethod(typeof(Child), typeof(Parent), typeof(IEnumerable<Child>)),
            includeCall,
            SystemLinq.Expression.Quote(collectionNav));

        var systemCall = thenIncludeCall.ReplaceIncludeQueryMethodsByStringInclude().ShouldBeAssignableTo<SystemLinq.MethodCallExpression>();

        AssertSystemStringIncludeCall(systemCall, typeof(Child), systemQueryable, "Parent.Children");
    }

    [Fact]
    public void Should_convert_collection_then_include_to_string_include()
    {
        // Remote side: a collection navigation (Parent.Children) followed by a
        // collection-based ThenInclude (Child.Parent) must be collapsed into a
        // single string-include call.
        var provider = CreateProvider();
        var queryable = RemoteQueryable.Factory.CreateQueryable<Parent>(provider.DataProvider);
        _ = queryable
            .Include(x => x.Children)
            .ThenInclude(x => x.Parent)
            .ToList();
        var remoteTree = provider.LastExpression.ShouldNotBeNull();

        var rewritten = remoteTree.ReplaceIncludeQueryMethodsByStringInclude();

        var call = rewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteStringIncludeCall(call, typeof(Parent), "Children.Parent");

        // System side: the collection-based ThenInclude chain must be rewritten
        // to the equivalent string-include call over the original queryable.
        var systemQueryable = new[] { new Parent() }.AsQueryable();
        SystemLinq.Expression<Func<Parent, IEnumerable<Child>>> collectionNav = x => x.Children;
        SystemLinq.Expression<Func<Child, Parent>> referenceNav = x => x.Parent;
        var includeCall = SystemLinq.Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Parent), typeof(IEnumerable<Child>)),
            SystemLinq.Expression.Constant(systemQueryable),
            SystemLinq.Expression.Quote(collectionNav));
        var thenIncludeCall = SystemLinq.Expression.Call(
            IncludeQueryableExtensions.ThenIncludeAfterEnumerableMethodInfo.MakeGenericMethod(typeof(Parent), typeof(Child), typeof(Parent)),
            includeCall,
            SystemLinq.Expression.Quote(referenceNav));

        var systemCall = thenIncludeCall.ReplaceIncludeQueryMethodsByStringInclude().ShouldBeAssignableTo<SystemLinq.MethodCallExpression>();

        AssertSystemStringIncludeCall(systemCall, typeof(Parent), systemQueryable, "Children.Parent");
    }

    [Fact]
    public void Should_convert_multiple_includes_without_retaining_stacked_queryables()
    {
        // Two independent include chains over the same remote queryable. During
        // the rewrite each chain is tracked in a stacked includable queryable
        // constant, and the cleaner must eliminate every stacked queryable,
        // leaving only nested string-include calls over the resource descriptor.
        var provider = CreateProvider();
        var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(provider.DataProvider);
        _ = queryable
            .Include(x => x.Parent)
            .ThenInclude(x => x.Children)
            .Include(x => x.Siblings)
            .ThenInclude(x => x.Siblings)
            .ToList();
        var remoteTree = provider.LastExpression.ShouldNotBeNull();

        var rewritten = remoteTree.ReplaceIncludeQueryMethodsByStringInclude();

        var outer = rewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        outer.Method.Name.ShouldBe("Include");
        outer.Arguments!.Count.ShouldBe(2);
        outer.Arguments[1].ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBe("Siblings.Siblings");

        var inner = outer.Arguments[0].ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteStringIncludeCall(inner, typeof(Child), "Parent.Children");
        GetResourceDescriptorArgument(inner)
            .Value.ShouldBeOfType<QueryableResourceDescriptor>()
            .Type.ToType().ShouldBe(typeof(Child));

        // No stacked includable queryable may survive the rewrite: every
        // remaining constant is the resource descriptor or an include path string.
        CollectConstantValues(rewritten).ShouldAllBe(x => x is QueryableResourceDescriptor || x is string);
    }

    [Fact]
    public void Should_build_subselect_navigation_for_reference_and_collection_segments()
    {
        // Reference segment: the navigation body is a plain member-access chain
        // on the root parameter without any Enumerable.Select call.
        var referenceLambda = SystemIncludeExpressionReWriter.BuildSubSelectNavigation<Child>("Parent.Children");
        AssertReferenceSubSelectShape(referenceLambda);

        // Collection segment: the navigation body is an Enumerable.Select call
        // over the collection member whose lambda selects the nested navigation.
        var collectionLambda = SystemIncludeExpressionReWriter.BuildSubSelectNavigation<Parent>("Children.Parent");
        AssertCollectionSubSelectShape(collectionLambda);

        // Remote side: the subselect strategy must emit a lambda-include call
        // with a quoted remote lambda while retaining the string-include path
        // on the inner call.
        var referenceProvider = CreateProvider();
        var referenceQueryable = RemoteQueryable.Factory.CreateQueryable<Child>(referenceProvider.DataProvider);
        _ = referenceQueryable
            .Include(x => x.Parent)
            .ThenInclude(x => x.Children)
            .ToList();
        var referenceRewritten = referenceProvider.LastExpression.ShouldNotBeNull().ReplaceThenIncludeQueryMethodsBySubSelects(null);
        var referenceCall = referenceRewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteLambdaIncludeCall(referenceCall, typeof(Child), typeof(IEnumerable<Child>), "Parent.Children");
        AssertReferenceSubSelectShape(GetQuotedLambda(referenceCall));

        var collectionProvider = CreateProvider();
        var collectionQueryable = RemoteQueryable.Factory.CreateQueryable<Parent>(collectionProvider.DataProvider);
        _ = collectionQueryable
            .Include(x => x.Children)
            .ThenInclude(x => x.Parent)
            .ToList();
        var collectionRewritten = collectionProvider.LastExpression.ShouldNotBeNull().ReplaceThenIncludeQueryMethodsBySubSelects(null);
        var collectionCall = collectionRewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteLambdaIncludeCall(collectionCall, typeof(Parent), typeof(IEnumerable<Parent>), "Children.Parent");
        AssertCollectionSubSelectShape(GetQuotedLambda(collectionCall));
    }

    [Fact]
    public void Should_retain_non_include_method_calls()
    {
        var provider = CreateProvider();
        var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(provider.DataProvider);
        _ = queryable
            .Where(x => x.Siblings != null)
            .Include(x => x.Parent)
            .ToList();
        var remoteTree = provider.LastExpression.ShouldNotBeNull();

        var rewritten = remoteTree.ReplaceIncludeQueryMethodsByStringInclude();

        var call = rewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        AssertRemoteStringIncludeCall(call, typeof(Child), "Parent");

        // The Where call must be retained as the source of the string include,
        // keeping its method, resource argument, and predicate lambda.
        var whereCall = call.Arguments![0].ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        whereCall.Method.Name.ShouldBe("Where");
        whereCall.Method.DeclaringType.ToType().ShouldBe(typeof(Queryable));
        whereCall.Arguments!.Count.ShouldBe(2);
        whereCall.Arguments[0].ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBeOfType<QueryableResourceDescriptor>();
        var quotedPredicate = whereCall.Arguments[1].ShouldBeOfType<RemoteLinq.UnaryExpression>();
        quotedPredicate.UnaryOperator.ShouldBe(RemoteLinq.UnaryOperator.Quote);
        var predicate = quotedPredicate.Operand.ShouldBeOfType<RemoteLinq.LambdaExpression>();
        var comparison = predicate.Expression.ShouldBeOfType<RemoteLinq.BinaryExpression>();
        comparison.BinaryOperator.ShouldBe(RemoteLinq.BinaryOperator.NotEqual);

        // Without any include call the tree must be returned untouched.
        var untouchedProvider = CreateProvider();
        var untouchedQueryable = RemoteQueryable.Factory.CreateQueryable<Child>(untouchedProvider.DataProvider);
        _ = untouchedQueryable
            .Where(x => x.Siblings != null)
            .ToList();
        var untouchedTree = untouchedProvider.LastExpression.ShouldNotBeNull();

        var untouchedRewritten = untouchedTree.ReplaceIncludeQueryMethodsByStringInclude();

        var untouchedCall = untouchedRewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        untouchedCall.Method.Name.ShouldBe("Where");
        untouchedCall.Arguments!.Count.ShouldBe(2);
        CollectMethodCallNames(untouchedRewritten).ShouldNotContain("Include");
    }

    [Fact]
    public void Should_use_supplied_string_include_method_provider()
    {
        // Remote side: the supplied provider must replace the default
        // IncludeQueryableExtensions.Include(T, string) method.
        var provider = CreateProvider();
        var queryable = RemoteQueryable.Factory.CreateQueryable<Child>(provider.DataProvider);
        _ = queryable
            .Include(x => x.Parent)
            .ToList();
        var remoteTree = provider.LastExpression.ShouldNotBeNull();

        var rewritten = remoteTree.ReplaceIncludeQueryMethodsByStringInclude(type => CustomStringIncludeDefinition.MakeGenericMethod(type));

        var call = rewritten.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        call.Method.Name.ShouldBe(nameof(CustomStringInclude));
        call.Method.DeclaringType.ToType().ShouldBe(typeof(When_rewriting_include_paths));
        call.Arguments!.Count.ShouldBe(2);
        call.Arguments[1].ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBe("Parent");
        GetResourceDescriptorArgument(call).Value.ShouldBeOfType<QueryableResourceDescriptor>();

        // System side: the same custom method must be used for system trees.
        var systemQueryable = new[] { new Child() }.AsQueryable();
        SystemLinq.Expression<Func<Child, Parent>> referenceNav = x => x.Parent;
        var includeCall = SystemLinq.Expression.Call(
            IncludeQueryableExtensions.IncludeMethodInfo.MakeGenericMethod(typeof(Child), typeof(Parent)),
            SystemLinq.Expression.Constant(systemQueryable),
            SystemLinq.Expression.Quote(referenceNav));

        var systemCall = includeCall
            .ReplaceIncludeQueryMethodsByStringInclude(type => CustomStringIncludeDefinition.MakeGenericMethod(type))
            .ShouldBeAssignableTo<SystemLinq.MethodCallExpression>();

        systemCall.Method.Name.ShouldBe(nameof(CustomStringInclude));
        systemCall.Method.DeclaringType.ShouldBe(typeof(When_rewriting_include_paths));
        systemCall.Arguments.Count.ShouldBe(2);
        systemCall.Arguments[0].ShouldBeOfType<SystemLinq.ConstantExpression>().Value.ShouldBe(systemQueryable);
        systemCall.Arguments[1].ShouldBeOfType<SystemLinq.ConstantExpression>().Value.ShouldBe("Parent");
    }

    [Fact]
    public void Should_reject_empty_subselect_navigation_paths()
    {
        var emptyException = Record.Exception(() => SystemIncludeExpressionReWriter.BuildSubSelectNavigation<Parent>(string.Empty));
        emptyException.ShouldBeOfType<ArgumentException>();

        var nullException = Record.Exception(() => SystemIncludeExpressionReWriter.BuildSubSelectNavigation<Parent>(null!));
        nullException.ShouldBeOfType<ArgumentNullException>();
    }

    private static RecordingSyncProvider<DynamicObject> CreateProvider()
        => new(new DynamicObject(Array.Empty<object>()));

    private static void AssertRemoteStringIncludeCall(RemoteLinq.MethodCallExpression call, Type elementType, string includePath)
    {
        call.Method.Name.ShouldBe("Include");
        call.Method.DeclaringType.ToType().ShouldBe(typeof(IncludeQueryableExtensions));
        call.Method.GenericArgumentTypes!.Select(x => x.ToType()).ShouldBe([elementType]);
        call.Arguments!.Count.ShouldBe(2);
        call.Arguments[1].ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBe(includePath);
        GetResourceDescriptorArgument(call)
            .Value.ShouldBeOfType<QueryableResourceDescriptor>()
            .Type.ToType().ShouldBe(elementType);
    }

    private static void AssertSystemStringIncludeCall(SystemLinq.MethodCallExpression call, Type elementType, IQueryable queryable, string includePath)
    {
        call.Method.Name.ShouldBe("Include");
        call.Method.DeclaringType.ShouldBe(typeof(IncludeQueryableExtensions));
        call.Method.GetGenericArguments().ShouldBe([elementType]);
        call.Arguments.Count.ShouldBe(2);
        call.Arguments[0].ShouldBeOfType<SystemLinq.ConstantExpression>().Value.ShouldBe(queryable);
        call.Arguments[1].ShouldBeOfType<SystemLinq.ConstantExpression>().Value.ShouldBe(includePath);
    }

    private static void AssertRemoteLambdaIncludeCall(RemoteLinq.MethodCallExpression call, Type elementType, Type navigationType, string includePath)
    {
        call.Method.Name.ShouldBe("Include");
        call.Method.DeclaringType.ToType().ShouldBe(typeof(IncludeQueryableExtensions));
        call.Method.GenericArgumentTypes!.Select(x => x.ToType()).ShouldBe([elementType, navigationType]);
        call.Arguments!.Count.ShouldBe(2);
        var inner = call.Arguments[0].ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        inner.Method.Name.ShouldBe("Include");
        inner.Arguments![1].ShouldBeOfType<RemoteLinq.ConstantExpression>().Value.ShouldBe(includePath);
    }

    private static SystemLinq.LambdaExpression GetQuotedLambda(RemoteLinq.MethodCallExpression call)
    {
        var quoted = call.Arguments![1].ShouldBeOfType<RemoteLinq.UnaryExpression>();
        quoted.UnaryOperator.ShouldBe(RemoteLinq.UnaryOperator.Quote);
        return quoted.Operand.ToLinqExpression().ShouldBeAssignableTo<SystemLinq.LambdaExpression>();
    }

    private static void AssertReferenceSubSelectShape(SystemLinq.LambdaExpression lambda)
    {
        lambda.Parameters.Count.ShouldBe(1);
        lambda.Parameters[0].Type.ShouldBe(typeof(Child));
        var navigation = lambda.Body.ShouldBeAssignableTo<SystemLinq.MemberExpression>();
        navigation.Member.Name.ShouldBe("Children");
        var parentAccess = navigation.Expression!.ShouldBeAssignableTo<SystemLinq.MemberExpression>();
        parentAccess.Member.Name.ShouldBe("Parent");
        parentAccess.Expression!.NodeType.ShouldBe(SystemLinq.ExpressionType.Parameter);
    }

    private static void AssertCollectionSubSelectShape(SystemLinq.LambdaExpression lambda)
    {
        lambda.Parameters.Count.ShouldBe(1);
        lambda.Parameters[0].Type.ShouldBe(typeof(Parent));
        var select = lambda.Body.ShouldBeAssignableTo<SystemLinq.MethodCallExpression>();
        select.Method.Name.ShouldBe("Select");
        select.Method.DeclaringType.ShouldBe(typeof(Enumerable));
        select.Arguments.Count.ShouldBe(2);
        select.Arguments[0].ShouldBeAssignableTo<SystemLinq.MemberExpression>().Member.Name.ShouldBe("Children");
        var innerLambda = select.Arguments[1].ShouldBeAssignableTo<SystemLinq.LambdaExpression>();
        innerLambda.Parameters.Count.ShouldBe(1);
        innerLambda.Parameters[0].Type.ShouldBe(typeof(Child));
        innerLambda.Body.ShouldBeAssignableTo<SystemLinq.MemberExpression>().Member.Name.ShouldBe("Parent");
    }

    private static RemoteLinq.ConstantExpression GetResourceDescriptorArgument(RemoteLinq.MethodCallExpression call)
    {
        var argument = call.Arguments![0];
        while (argument is RemoteLinq.MethodCallExpression nestedCall)
        {
            argument = nestedCall.Arguments![0];
        }

        return argument.ShouldBeOfType<RemoteLinq.ConstantExpression>();
    }

    private static IEnumerable<object> CollectConstantValues(RemoteLinq.Expression expression)
    {
        if (expression is RemoteLinq.ConstantExpression constant)
        {
            yield return constant.Value;
        }

        switch (expression)
        {
            case RemoteLinq.MethodCallExpression call:
                if (call.Instance is not null)
                {
                    foreach (var value in CollectConstantValues(call.Instance))
                    {
                        yield return value;
                    }
                }

                if (call.Arguments is not null)
                {
                    foreach (var argument in call.Arguments)
                    {
                        foreach (var value in CollectConstantValues(argument))
                        {
                            yield return value;
                        }
                    }
                }

                break;

            case RemoteLinq.LambdaExpression lambda:
                foreach (var value in CollectConstantValues(lambda.Expression))
                {
                    yield return value;
                }

                break;

            case RemoteLinq.UnaryExpression unary:
                foreach (var value in CollectConstantValues(unary.Operand))
                {
                    yield return value;
                }

                break;

            case RemoteLinq.BinaryExpression binary:
                foreach (var value in CollectConstantValues(binary.LeftOperand))
                {
                    yield return value;
                }

                foreach (var value in CollectConstantValues(binary.RightOperand))
                {
                    yield return value;
                }

                break;

            case RemoteLinq.MemberExpression member:
                if (member.Expression is not null)
                {
                    foreach (var value in CollectConstantValues(member.Expression))
                    {
                        yield return value;
                    }
                }

                break;
        }
    }

    private static IEnumerable<string> CollectMethodCallNames(RemoteLinq.Expression expression)
    {
        if (expression is RemoteLinq.MethodCallExpression call)
        {
            yield return call.Method.Name!;
        }

        switch (expression)
        {
            case RemoteLinq.MethodCallExpression methodCall:
                if (methodCall.Instance is not null)
                {
                    foreach (var name in CollectMethodCallNames(methodCall.Instance))
                    {
                        yield return name;
                    }
                }

                if (methodCall.Arguments is not null)
                {
                    foreach (var argument in methodCall.Arguments)
                    {
                        foreach (var name in CollectMethodCallNames(argument))
                        {
                            yield return name;
                        }
                    }
                }

                break;

            case RemoteLinq.LambdaExpression lambda:
                foreach (var name in CollectMethodCallNames(lambda.Expression))
                {
                    yield return name;
                }

                break;

            case RemoteLinq.UnaryExpression unary:
                foreach (var name in CollectMethodCallNames(unary.Operand))
                {
                    yield return name;
                }

                break;

            case RemoteLinq.BinaryExpression binary:
                foreach (var name in CollectMethodCallNames(binary.LeftOperand))
                {
                    yield return name;
                }

                foreach (var name in CollectMethodCallNames(binary.RightOperand))
                {
                    yield return name;
                }

                break;

            case RemoteLinq.MemberExpression member:
                if (member.Expression is not null)
                {
                    foreach (var name in CollectMethodCallNames(member.Expression))
                    {
                        yield return name;
                    }
                }

                break;
        }
    }
}
