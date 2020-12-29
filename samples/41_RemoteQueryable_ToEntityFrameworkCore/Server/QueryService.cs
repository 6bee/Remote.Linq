// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.EntityFrameworkCore;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        public async Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
        {
            ////try
            ////{
                using var efContext = new EFContext();
                return await queryExpression.ExecuteWithEntityFrameworkCoreAsync(efContext, cancellation).ConfigureAwait(false);
            ////}
            ////catch (DbException ex)
            ////{
            ////    throw new Exception($"{ex.GetType()}: {ex.Message}");
            ////}
        }
    }
}
