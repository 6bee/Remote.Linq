// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Common.ServiceContract
{
    using Common.Model;
    using System.Collections.Generic;
    using System.ServiceModel;

    [ServiceContract]
    public interface ITraditionalDataService
    {
        [OperationContract]
        IEnumerable<Product> GetProductsByName(string productName);

        [OperationContract]
        IEnumerable<Order> GetOrdersByProductId(long productId);
    }
}