// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using Remote.Linq.EntityFramework;
    using System.Collections.Generic;

    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            using (var efContext = new EFContext())
            {
                var result = queryExpression.ExecuteWithEntityFramework(efContext);
                return result;
            }
        }
    }
}
