// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Core;

using Remote.Linq;
using System.Reflection;
using SystemLinq = System.Linq.Expressions;

public class When_using_core_expression_utilities
{
    [Fact]
    public void Should_roundtrip_typed_lambda_expression()
    {
        SystemLinq.Expression<Func<int, bool>> expression = value => value > 3;

        var result = expression.ToRemoteLinqExpression().ToLinqExpression<Func<int, bool>>().Compile();

        result(4).ShouldBeTrue();
        result(3).ShouldBeFalse();
    }

    [Fact]
    public void Should_apply_predicate_combinations_when_predicates_are_present_or_missing()
    {
        Func<int, bool> isEven = value => value % 2 == 0;
        Func<int, bool> isPositive = value => value > 0;

        ((Func<int, bool>)null).And(isEven).ShouldBeSameAs(isEven);
        isEven.And(null).ShouldBeSameAs(isEven);
        isEven.And(isPositive)!(2).ShouldBeTrue();
        isEven.And(isPositive)!(-2).ShouldBeFalse();
        ((Func<int, bool>)null).Or(isEven).ShouldBeSameAs(isEven);
        isEven.Or(null).ShouldBeSameAs(isEven);
        isEven.Or(isPositive)!(-1).ShouldBeFalse();
        isEven.Or(isPositive)!(1).ShouldBeTrue();
    }

    [Fact]
    public void Should_enumerate_grouping_and_reject_missing_elements()
    {
        var grouping = new Grouping<string, int> { Key = "key", Elements = [1, 2] };

        grouping.Key.ShouldBe("key");
        grouping.Cast<int>().ShouldBe([1, 2]);
        Should.Throw<InvalidOperationException>(() => new Grouping<string, int>().ToList());
    }

    [Fact]
    public void Should_create_type_and_method_information_for_values_and_nulls()
    {
        typeof(string).AsTypeInfo().ToType().ShouldBe(typeof(string));
        ((Type)null).AsTypeInfo().ShouldBeNull();

        var method = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
        method.AsMethodInfo()!.Name.ShouldBe(nameof(string.StartsWith));
        ((MethodInfo)null).AsMethodInfo().ShouldBeNull();
    }

    [Fact]
    public void Should_unwrap_exceptions_from_method_and_delegate_invocation()
    {
        var method = typeof(When_using_core_expression_utilities).GetMethod(nameof(ThrowInvalidOperation), BindingFlags.Static | BindingFlags.NonPublic)!;
        var exception = Should.Throw<InvalidOperationException>(() => method.InvokeAndUnwrap(null));
        exception.Message.ShouldBe("failure");

        Action action = ThrowInvalidOperation;
        Should.Throw<InvalidOperationException>(() => action.DynamicInvokeAndUnwrap()).Message.ShouldBe("failure");
    }

    [Fact]
    public void Should_provide_stable_factories_and_exception_constructors()
    {
        RemoteQueryable.Factory.ToString().ShouldBe("Remote.Linq.RemoteQueryable.Factory");
        RemoteQueryable.Factory.Equals(new object()).ShouldBeFalse();
        RemoteQueryable.Factory.Equals(RemoteQueryable.Factory).ShouldBeTrue();
        RemoteQueryable.Factory.GetHashCode().ShouldBe(0);
        SystemExpression.Factory.ToString().ShouldBe("Remote.Linq.SystemExpression.Factory");
        SystemExpression.Factory.Equals(SystemExpression.Factory).ShouldBeTrue();
        SystemExpression.Factory.GetHashCode().ShouldBe(0);

        new RemoteLinqException().Message.ShouldNotBeNull();
        new RemoteLinqException("message").Message.ShouldBe("message");
        new RemoteLinqException("message", new ArgumentException("inner")).InnerException.ShouldBeOfType<ArgumentException>();
    }

    private static void ThrowInvalidOperation() => throw new InvalidOperationException("failure");
}
