// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContract
{
    using Common.Model;
    using Remote.Linq;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface IRemoteLinqDataService
    {
        [OperationContract]
        IEnumerable<Product> GetProducts(Query<Product> query);

        [OperationContract]
        IEnumerable<Order> GetOrders(Query<Order> query);

        [OperationContract]
        [ServiceKnownType(typeof(Query))]
        [ServiceKnownType(typeof(Product))]
        [ServiceKnownType(typeof(Order))]
        IEnumerable<object> GetData(IQuery query);
    }
}
