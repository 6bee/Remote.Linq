// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.QueryMarkerFunctionAttribute;

using Aqua.Dynamic;
using Remote.Linq.DynamicQuery;
using Shouldly;
using Xunit;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

public class When_annotating_method
{
    public class LocalClass
    {
        public int Method() => 1234;

        [QueryMarkerFunction]
        public int QueryFunction() => 2;
    }

    [Fact]
    public void Should_evaluate_method_invocation()
    {
        var exp = SystemLinq.Expression.Call(
            SystemLinq.Expression.Constant(new LocalClass()),
            typeof(LocalClass).GetMethod(nameof(LocalClass.Method)));

        var rlinq = exp.ToRemoteLinqExpression();

        var newExp = rlinq.ShouldBeOfType<RemoteLinq.MemberExpression>()
            .Expression.ShouldBeOfType<RemoteLinq.NewExpression>();
        newExp.Constructor.ToConstructorInfo().DeclaringType.ShouldBe(typeof(VariableQueryArgument<int>));
        newExp.Arguments.ShouldHaveSingleItem()
            .ShouldBeOfType<RemoteLinq.ConstantExpression>()
            .Value.ShouldBe(1234);
    }

    [Fact]
    public void Should_prevent_local_evaluation_of_annotated_method()
    {
        var i = new LocalClass();
        var m = typeof(LocalClass).GetMethod(nameof(LocalClass.QueryFunction));
        var exp = SystemLinq.Expression.Call(SystemLinq.Expression.Constant(i), m);

        var rlinq = exp.ToRemoteLinqExpression();

        var call = rlinq.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
        call.Arguments.ShouldBeEmpty();
        call.Method.ToMethodInfo().ShouldBe(m);
        call.Instance.ShouldBeOfType<RemoteLinq.ConstantExpression>()
            .Value.ShouldBeOfType<ConstantQueryArgument>()
            .Value.ShouldBeOfType<DynamicObject>()
            .Type.ToType().ShouldBe(typeof(LocalClass));
    }
}