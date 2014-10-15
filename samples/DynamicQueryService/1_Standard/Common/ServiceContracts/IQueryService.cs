// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common.ServiceContracts
{
    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression);
    }
}
