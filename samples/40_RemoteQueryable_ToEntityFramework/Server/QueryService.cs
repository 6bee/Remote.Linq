// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.EntityFramework;
    using Remote.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        public async ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
        {
            using var efContext = new EFContext();
            return await queryExpression.ExecuteWithEntityFrameworkAsync(efContext, cancellation).ConfigureAwait(false);
        }
    }
}
