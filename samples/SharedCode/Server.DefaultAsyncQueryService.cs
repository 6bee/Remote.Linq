// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
            => Task.Run(() => queryExpression.Execute(DataStore.QueryableByTypeProvider), cancellation);
    }
}
