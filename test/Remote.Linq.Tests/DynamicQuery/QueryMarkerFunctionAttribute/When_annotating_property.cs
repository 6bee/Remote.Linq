// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.DynamicQuery.QueryMarkerFunctionAttribute
{
    using Remote.Linq.DynamicQuery;
    using Shouldly;
    using Xunit;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public class When_annotating_property
    {
        public class LocalClass
        {
            public int P => 1234;
        }

        [Fact]
        public void Should_evaluate_property_access()
        {
            var exp = SystemLinq.Expression.MakeMemberAccess(
                SystemLinq.Expression.Constant(new LocalClass()),
                typeof(LocalClass).GetProperty(nameof(LocalClass.P)));

            var rlinq = exp.ToRemoteLinqExpression();

            var newExp = rlinq.ShouldBeOfType<RemoteLinq.MemberExpression>()
                .Expression.ShouldBeOfType<RemoteLinq.NewExpression>();
            newExp.Constructor.ToConstructorInfo().DeclaringType.ShouldBe(typeof(VariableQueryArgument<int>));
            newExp
                .Arguments.ShouldHaveSingleItem()
                .ShouldBeOfType<RemoteLinq.ConstantExpression>()
                .Value.ShouldBe(1234);
        }
    }
}