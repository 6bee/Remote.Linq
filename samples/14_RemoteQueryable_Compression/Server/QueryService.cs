// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;

    public class QueryService : IQueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public byte[] ExecuteQuery(Expression queryExpression)
        {
            DynamicObject result = queryExpression.Execute(DataStore.QueryableByTypeProvider);

            byte[] compressedData = new CompressionHelper().Compress(result);

            return compressedData;
        }
    }
}
