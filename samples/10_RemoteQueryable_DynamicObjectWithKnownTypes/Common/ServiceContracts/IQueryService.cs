// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContracts
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq.Expressions;
    using System.ServiceModel;

    [ServiceContract]
    public interface IQueryService
    {
        [OperationContract]
        [ServiceKnownType(typeof(Product))]
        [ServiceKnownType(typeof(ProductGroup))]
        DynamicObject ExecuteQuery(Expression queryExpression);
    }
}