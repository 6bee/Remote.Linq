// Copyright (c) Christof Senn. All rights reserved. 

using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System.Collections.Generic;
using WcfContracts;

namespace WcfService
{
    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            var efContext = new EFContext();
            var result = queryExpression.Execute(queryableProvider: type => efContext.Set(type));
            return result;
        }
    }
}
