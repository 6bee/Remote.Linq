// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using Remote.Linq.TestSupport;
    using System.Collections.Generic;
    using System.Threading;

    public class QueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public IAsyncEnumerable<DynamicObject> ExecuteAsyncStreamQuery(Expression queryExpression, CancellationToken cancellation)
        {
            // DEMO: for demo purpose we fetch query result into memory
            // and use test-support extension method to return an asyn stream
            var result = queryExpression.Execute(DataStore.QueryableByTypeProvider);
            return result.AsAsyncRemoteStreamQueryable().ExecuteAsyncRemoteStream(cancellation);
        }
    }
}
