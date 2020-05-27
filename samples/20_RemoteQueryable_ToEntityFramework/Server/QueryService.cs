// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class QueryService : IQueryService
    {
        public async Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression)
        {
            using EFContext efContext = new EFContext();
            return await queryExpression.ExecuteWithEntityFrameworkAsync(efContext).ConfigureAwait(false);
        }
    }
}
