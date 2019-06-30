// Copyright (c) Christof Senn. All rights reserved. 

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using Remote.Linq.EntityFramework;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class QueryService : IQueryService
    {
        public async Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression)
        {
            using (var efContext = new EFContext())
            {
                return await queryExpression.ExecuteWithEntityFrameworkAsync(efContext).ConfigureAwait(false);
            }
        }
    }
}
