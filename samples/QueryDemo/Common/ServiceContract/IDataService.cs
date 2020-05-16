// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContract
{
    using Common.DataContract;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        IEnumerable<Product> GetProductsByName(string productName);

        [OperationContract]
        IEnumerable<Order> GetOrdersByProductId(long productId);
    }
}
