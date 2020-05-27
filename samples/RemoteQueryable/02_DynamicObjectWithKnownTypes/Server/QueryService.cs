// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class QueryService : IQueryService
    {
        private readonly Func<Type, IQueryable> _queryableResourceProvider = InMemoryDataStore.Instance.GetQueryableByType;

        private readonly DynamicObjectMapper _mapper = new DynamicObjectMapper(isKnownTypeProvider: new IsKnownTypeProvider());

        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
            => queryExpression.Execute(_queryableResourceProvider, mapper: _mapper);
    }
}
