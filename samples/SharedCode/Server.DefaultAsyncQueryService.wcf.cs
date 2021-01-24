// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System.Threading.Tasks;

    public class QueryService : IQueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public Task<DynamicObject> ExecuteQueryAsync(Expression queryExpression)
            => Task.Run(() => queryExpression.Execute(DataStore.QueryableByTypeProvider));
    }
}