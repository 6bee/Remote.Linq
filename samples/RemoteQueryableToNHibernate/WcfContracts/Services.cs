// Copyright (c) Christof Senn. All rights reserved. 

using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System.Collections.Generic;
using System.ServiceModel;

namespace WcfContracts
{
    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        IEnumerable<DynamicObject> ExecuteQuery(Expression queryExpression);
    }
}
