// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class QueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public async ValueTask<DynamicObject> ExecuteQueryAsync(Expression queryExpression, CancellationToken cancellation)
            => await Task.Run(() => queryExpression.Execute(DataStore.QueryableByTypeProvider), cancellation).ConfigureAwait(false);
    }
}