// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContracts
{
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression);
    }
}
