// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common.Model;
using Common.ServiceContracts;
using Remote.Linq;
using Remote.Linq.Expressions;
using System;
using System.Linq;
using System.ServiceModel;

namespace Client
{
    public class RemoteRepository
    {
        private readonly Func<Expression, object> _dataProvider;

        public RemoteRepository(string uri)
        {
            _dataProvider = expression =>
            {
                var binding = new NetNamedPipeBinding()
                {
                    CloseTimeout = TimeSpan.FromMinutes(10),
                    ReceiveTimeout = TimeSpan.FromMinutes(10),
                    SendTimeout = TimeSpan.FromMinutes(10),
                    MaxReceivedMessageSize = 640000L
                };
                var channelFactory = new ChannelFactory<IQueryService>(binding, uri);
                var channel = channelFactory.CreateChannel();
                
                var result = channel.ExecuteQuery(expression);
                return result;
            };
        }

        public IQueryable<ProductCategory> ProductCategories { get { return RemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        public IQueryable<Product> Products { get { return RemoteQueryable.Create<Product>(_dataProvider); } }
        public IQueryable<OrderItem> OrderItems { get { return RemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
