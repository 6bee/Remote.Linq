// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.QueryArgumentAttribute
{
    using Aqua.Dynamic;
    using Remote.Linq.DynamicQuery;
    using Shouldly;
    using Xunit;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public class When_annotating_struct
    {
        [QueryArgument]
        public struct QArg
        {
            public QArg(int v)
                => V = v;

            public int V { get; }
        }

        public struct Q
        {
            public Q(int v)
                => V = v;

            public int V { get; }
        }

        [Fact]
        public void Should_prevent_local_evaluation_of_annotated_type()
        {
            var i = new QArg(1234);
            var p = typeof(QArg).GetProperty(nameof(QArg.V));
            var exp = SystemLinq.Expression.MakeMemberAccess(SystemLinq.Expression.Constant(i), p);

            var rlinq = exp.ToRemoteLinqExpression();

            var access = rlinq.ShouldBeOfType<RemoteLinq.MemberExpression>();
            access.Member.ToMemberInfo().ShouldBe(p);
            access.Expression.ShouldBeOfType<RemoteLinq.ConstantExpression>()
                .Value.ShouldBeOfType<ConstantQueryArgument>()
                .Value.ShouldBeOfType<DynamicObject>()
                .Type.ToType().ShouldBe(typeof(QArg));
        }

        [Fact]
        public void Should_evaluation_local_type()
        {
            var i = new Q(1234);
            var p = typeof(Q).GetProperty(nameof(Q.V));
            var exp = SystemLinq.Expression.MakeMemberAccess(SystemLinq.Expression.Constant(i), p);

            var rlinq = exp.ToRemoteLinqExpression();

            rlinq.ShouldBeOfType<RemoteLinq.MemberExpression>()
                .Expression.ShouldBeOfType<RemoteLinq.NewExpression>()
                .Arguments.ShouldHaveSingleItem()
                .ShouldBeOfType<RemoteLinq.ConstantExpression>()
                .Value.ShouldBe(1234);
        }
    }
}