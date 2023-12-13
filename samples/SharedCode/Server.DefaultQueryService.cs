// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;

    public partial class QueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        public DynamicObject ExecuteQuery(Expression queryExpression)
            => queryExpression.Execute(DataStore.QueryableByTypeProvider);
    }
}