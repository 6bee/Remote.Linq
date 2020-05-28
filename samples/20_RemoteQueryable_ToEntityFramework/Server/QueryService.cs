// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class QueryService
    {
        public async Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression)
        {
            using var efContext = new EFContext();
            try
            {
                var result = await queryExpression.ExecuteWithEntityFrameworkAsync(efContext).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
