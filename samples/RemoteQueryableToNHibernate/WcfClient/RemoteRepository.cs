// Copyright (c) Christof Senn. All rights reserved. 

namespace WcfClient
{
    using Remote.Linq;
    using Remote.Linq.Dynamic;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using WcfContracts;

    public class RemoteRepository
    {
        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

        public RemoteRepository(string address)
        {
            _dataProvider = expression =>
            {
                var binding = new BasicHttpBinding()
                {
                    CloseTimeout = TimeSpan.FromMinutes(10),
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(10),
                    MaxReceivedMessageSize = 640000L
                };
                var channelFactory = new ChannelFactory<IQueryService>(binding, address);
                var channel = channelFactory.CreateChannel();

                var data = channel.ExecuteQuery(expression);
                return data;
            };
        }

        public IQueryable<ProductCategory> ProductCategories { get { return RemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        
        public IQueryable<Product> Products { get { return RemoteQueryable.Create<Product>(_dataProvider); } }
        
        public IQueryable<OrderItem> OrderItems { get { return RemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
