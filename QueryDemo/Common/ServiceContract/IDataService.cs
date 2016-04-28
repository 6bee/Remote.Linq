// Copyright (c) Christof Senn. All rights reserved. 

using System.Collections.Generic;
using System.ServiceModel;
using Common.DataContract;

namespace Common.ServiceContract
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        IEnumerable<Product> GetProductsByName(string productName);

        [OperationContract]
        IEnumerable<Order> GetOrdersByProductId(long productId);
    }
}
