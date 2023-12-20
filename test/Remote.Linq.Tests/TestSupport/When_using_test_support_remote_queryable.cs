// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.TestSupport;

using Remote.Linq.Async;
using Remote.Linq.TestSupport;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class When_using_test_support_remote_queryable
{
    private readonly IQueryable<double> _queryable;

    public When_using_test_support_remote_queryable()
    {
        _queryable = new[] { 3.14159, 2.718 }.AsRemoteQueryable();
    }

    [Fact]
    public void Queryable_should_be_async_remote_queryable_type()
    {
        _queryable
            .ShouldBeAssignableTo<IAsyncRemoteQueryable<double>>()
            .Provider.Execute<IEnumerable<double>>(_queryable.Expression)
            .ShouldBe(_queryable);
    }

    [Fact]
    public void Subsequently_calling_AsRemoteQueryable_should_return_same_instace()
    {
        var remoteAsyncQueryable = _queryable.AsRemoteQueryable();
        remoteAsyncQueryable.ShouldBeSameAs(_queryable);
    }

    [Fact]
    public async Task Should_allow_async_execution_of_remote_queryable_created_with_test_support_method()
    {
        var result = await Enumerable.Range(1, 100).AsRemoteQueryable().SumAsync();
        result.ShouldBe(5050);
    }

    [Fact]
    public async Task Should_allow_joining_remote_queryable_created_with_test_support_method()
    {
        var factor1 = 2.55;
        var factor2 = 0.55;
        var factors = new[] { factor1, factor2 };

        var result = await (
                from x in _queryable
                from f in factors
                select x * f)
            .SumAsync();

        result.ShouldBe((_queryable.Sum() * factor1) + (_queryable.Sum() * factor2), 0.0000001);
    }
}