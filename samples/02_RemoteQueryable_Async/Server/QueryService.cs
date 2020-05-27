// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class QueryService : IQueryService
    {
        private readonly Func<Type, IQueryable> _queryableResourceProvider = InMemoryDataStore.Instance.GetQueryableByType;

        public Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression)
            => Task.Run(() => queryExpression.Execute(_queryableResourceProvider));
    }
}
