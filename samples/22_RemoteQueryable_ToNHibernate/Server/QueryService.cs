// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        public Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
        {
            using NHContext nhContext = new NHContext();
            return Task.FromResult(queryExpression.Execute(nhContext.GetQueryable));
        }
    }
}
