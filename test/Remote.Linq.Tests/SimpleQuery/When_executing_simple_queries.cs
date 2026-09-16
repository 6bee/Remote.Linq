// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.SimpleQuery;

using Remote.Linq.SimpleQuery;
using System;
using System.Linq;

public class When_executing_simple_queries
{
    [Fact]
    public void Should_apply_filters_sorting_and_paging_to_enumerables()
    {
        var filtered = new Query<int>().Where(value => value > 1);
        var ordered = filtered.OrderByDescending(value => value);
        var query = ordered.Take(2);

        var result = new[] { 1, 2, 3, 4 }.ApplyQuery(query).ToArray();

        result.ShouldBe([4, 3]);
        query.HasFilters.ShouldBeTrue();
        query.HasSorting.ShouldBeTrue();
        query.HasPaging.ShouldBeTrue();
    }

    [Fact]
    public void Should_chain_sorting_and_reject_then_by_without_ordering()
    {
        var query = (IOrderedQuery<int>)new Query<int>().OrderBy(value => value % 2);
        var ordered = query.ThenBy(value => value);

        new[] { 3, 2, 1, 4 }.ApplyQuery(ordered).ShouldBe([2, 4, 1, 3]);
        Should.Throw<InvalidOperationException>(() => ((IOrderedQuery<int>)new Query<int>()).ThenBy(value => value));
        Should.Throw<InvalidOperationException>(() => ((IOrderedQuery<int>)new Query<int>()).ThenByDescending(value => value));
    }

    [Fact]
    public void Should_execute_data_provider_and_reject_enumeration_without_one()
    {
        var query = new Query<int>(_ => [1, 2]);

        query.ShouldBe([1, 2]);
        Should.Throw<RemoteLinqException>(() => new Query<int>().ToList());
    }

    [Fact]
    public void Should_convert_non_generic_query_when_the_type_matches()
    {
        var query = new Query(typeof(int), take: 1);

        var generic = query.ToGenericQuery<int>();

        generic.TakeValue.ShouldBe(1);
        Should.Throw<RemoteLinqException>(() => query.ToGenericQuery<string>());
    }
}
