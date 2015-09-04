// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService
{
    using Aqua.Dynamic;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using WcfContracts;

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
