// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.EntityFrameworkCore.Tests
{
    using Aqua.Dynamic;
    using Microsoft.EntityFrameworkCore;
    using Remote.Linq;
    using Remote.Linq.EntityFrameworkCore.Tests.Model;
    using Remote.Linq.Expressions;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DbFunctionsTest
    {
        [Fact]
        public void Should_not_evaluate_ef_functions_prematurely()
        {
            var source = Enumerable.Range(0, 100).Select(i => new Item { Name = i.ToString().PadLeft(2, '0') }).AsQueryable();

            Func<Expression, IEnumerable<DynamicObject>> execute =
                expression => expression.ExecuteWithEntityFrameworkCore(t => source);

            var queryable = RemoteQueryable.Factory.CreateEntityFrameworkCoreQueryable<Item>(execute);

            var result = queryable.Where(p => EF.Functions.Like(p.Name, "1%")).ToList();

            result.Count().ShouldBe(10);
        }
    }
}
