// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContracts
{
    using Aqua.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        Task<IEnumerable<DynamicObject>> ExecuteQueryAsync(Expression queryExpression);
    }
}
