// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.IEnumerableAsyncExtensions
{
    using Shouldly;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class When_using_async_with_non_async_queriables
    {
        private int[] array = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 1001, 1002, 1004, -1, -2, -5 };

        [Fact]
        public async Task Should_execute_synchronously()
        {
            var queryable = array.AsQueryable();
            var query =
                from item in queryable
                orderby item
                select item;
            var result = await query.ToListAsync();

            result.Count.ShouldBe(array.Length);
        }
    }
}
