// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;

    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            try
            {
                var efContext = new EFContext();
                var result = queryExpression.ExecuteWithEntityFramework(efContext);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
