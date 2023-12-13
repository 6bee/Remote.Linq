// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContracts
{
    using Remote.Linq.Expressions;
    using System.ServiceModel;

    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        byte[] ExecuteQuery(Expression queryExpression);
    }
}