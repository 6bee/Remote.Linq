// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.ExpressionExecution;
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
            var resultSet = ExecuteQuery(queryExpression);

            return resultSet.AsAsyncRemoteStreamQueryable().ExecuteAsyncRemoteStream(cancellation);
        }

        private IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            var result = queryExpression.Execute(DataStore.QueryableByTypeProvider);
            return ReMapCollection(result);
        }

        private static IEnumerable<DynamicObject> ReMapCollection(DynamicObject dynamicObject)
        {
            var mapper = new DynamicObjectMapper();
            var items = mapper.Map(dynamicObject);
            return mapper.MapCollection(items);
        }
    }
}