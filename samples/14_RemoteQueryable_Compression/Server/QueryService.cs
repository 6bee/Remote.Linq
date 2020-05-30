// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;

    public class QueryService : IQueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public byte[] ExecuteQuery(Expression queryExpression)
        {
            IEnumerable<Aqua.Dynamic.DynamicObject> result = queryExpression.Execute(DataStore.QueryableByTypeProvider);

            byte[] compressedData = new CompressionHelper().Compress(result);

            return compressedData;
        }
    }
}
