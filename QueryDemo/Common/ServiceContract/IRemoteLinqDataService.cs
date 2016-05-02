// Copyright (c) Christof Senn. All rights reserved. 

using System.Collections.Generic;
using System.ServiceModel;
using Common.DataContract;
using Remote.Linq;

namespace Common.ServiceContract
{
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
