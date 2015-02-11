// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContracts
{
    using Common.Model;
    using Remote.Linq.Expressions;
    using System.ServiceModel;

    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        [ServiceKnownType(typeof(Product[]))]
        [ServiceKnownType(typeof(string[]))]
        [ServiceKnownType(typeof(int[]))]
        [ServiceKnownType(typeof(decimal))]
        object ExecuteQuery(Expression queryExpression);
    }
}
