// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfService
{
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using WcfContracts;

    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            using (var nhContext = new NHContext())
            {
                var result = queryExpression.Execute(queryableProvider: type => nhContext.GetQueryable(type));
                return result;
            }
        }
    }
}
