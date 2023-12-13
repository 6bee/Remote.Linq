// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Aqua.Dynamic;
    using Common;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.ExpressionExecution;
    using Remote.Linq.Expressions;

    public class QueryService : IQueryService
    {
        private InMemoryDataStore DataStore => InMemoryDataStore.Instance;

        private IExpressionTranslatorContext Context => new ExpressionTranslatorContext(isKnownTypeProvider: new IsKnownTypeProvider());

        public DynamicObject ExecuteQuery(Expression queryExpression)
            => queryExpression.Execute(DataStore.QueryableByTypeProvider, Context);
    }
}