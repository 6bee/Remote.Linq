// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;

    public class QueryService : IQueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public object ExecuteQuery(Expression queryExpression)
            => queryExpression.Execute<object>(DataStore.QueryableByTypeProvider);
    }
}
