// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.QueryArgumentAttribute
{
    using Aqua.Dynamic;
    using Remote.Linq.DynamicQuery;
    using Shouldly;
    using Xunit;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public class When_annotating_class
    {
        [QueryArgument]
        public class QArg
        {
            public int Method() => 1;
        }

        public class Q
        {
            public int Method() => 1234;
        }

        [Fact]
        public void Should_prevent_local_evaluation_of_annotated_type()
        {
            var i = new QArg();
            var m = typeof(QArg).GetMethod(nameof(QArg.Method));
            var exp = SystemLinq.Expression.Call(SystemLinq.Expression.Constant(i), m);

            var rlinq = exp.ToRemoteLinqExpression();

            var call = rlinq.ShouldBeOfType<RemoteLinq.MethodCallExpression>();
            call.Arguments.ShouldBeEmpty();
            call.Method.ToMethodInfo().ShouldBe(m);
            call.Instance.ShouldBeOfType<RemoteLinq.ConstantExpression>()
                .Value.ShouldBeOfType<ConstantQueryArgument>()
                .Value.ShouldBeOfType<DynamicObject>()
                .Type.ToType().ShouldBe(typeof(QArg));
        }

        [Fact]
        public void Should_evaluation_local_type()
        {
            var i = new Q();
            var m = typeof(Q).GetMethod(nameof(Q.Method));
            var exp = SystemLinq.Expression.Call(SystemLinq.Expression.Constant(i), m);

            var rlinq = exp.ToRemoteLinqExpression();

            rlinq.ShouldBeOfType<RemoteLinq.MemberExpression>()
                .Expression.ShouldBeOfType<RemoteLinq.NewExpression>()
                .Arguments.ShouldHaveSingleItem()
                .ShouldBeOfType<RemoteLinq.ConstantExpression>()
                .Value.ShouldBe(1234);
        }
    }
}