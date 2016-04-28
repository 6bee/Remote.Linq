// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Common.ServiceContracts;
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;

    public class QueryService : IQueryService
    {
        public IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression)
        {
            try
            {
                var dataStore = InMemoryDataStore.Instance;

                return queryExpression.Execute(
                    queryableProvider: type => dataStore.GetSet(type),
                    typeResolver: new CustomTypeResolver());
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
