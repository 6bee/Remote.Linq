// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;

    public class QueryService : IQueryService
    {
        public object ExecuteQuery(Expression queryExpression) => new CustomExpressionExecutor().Execute(queryExpression);
    }
}
