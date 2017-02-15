// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.RemoteQueryable
{
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Linq;
    using Xunit;

    public class When_running_cross_join_query
    {
        class A
        {
        }

        class B
        {
        }

        public static IQueryable GetQueryableByType(Type type)
        {
            if (type == typeof(A))
            {
                return new[] { new A() }.AsQueryable();
            }

            if (type == typeof(B))
            {
                return new[] { new B() }.AsQueryable();
            }

            throw new NotSupportedException();
        }

        [Fact]
        public void Should_return_cross_product()
        {
            var queryableA = RemoteQueryable.Create<A>(x => x.Execute(queryableProvider: GetQueryableByType));

            var queryableB = RemoteQueryable.Create<B>(x => x.Execute(queryableProvider: GetQueryableByType));
            
            var query =
                from a in queryableA
                from b in queryableB
                select new { A = a, B = b };

            var result = query.Single();

            result.A.ShouldNotBeNull();

            result.B.ShouldNotBeNull();
        }
    }
}