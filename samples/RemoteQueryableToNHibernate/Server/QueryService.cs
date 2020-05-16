// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;

    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            using NHContext nhContext = new NHContext();
            return queryExpression.Execute(queryableProvider: type => nhContext.GetQueryable(type));
        }
    }
}
