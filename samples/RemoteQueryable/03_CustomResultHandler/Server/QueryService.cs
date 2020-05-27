// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContracts;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;

    public class QueryService : IQueryService
    {
        private readonly Func<Type, IQueryable> _queryableResourceProvider = InMemoryDataStore.Instance.GetQueryableByType;

        public object ExecuteQuery(Expression queryExpression)
        {
            try
            {
                var result = queryExpression.Execute<object>(_queryableResourceProvider);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
