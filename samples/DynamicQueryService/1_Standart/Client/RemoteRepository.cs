// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common.Model;
using Common.ServiceContracts;
using Remote.Linq;
using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Client
{
    public class RemoteRepository
    {
        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;

        public RemoteRepository(string uri)
        {
            _dataProvider = async expression =>
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

                var task = channel.ExecuteQueryAsync(expression);
                return await task;
            };
        }

        public IQueryable<ProductCategory> ProductCategories { get { return AsyncRemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        public IQueryable<Product> Products { get { return AsyncRemoteQueryable.Create<Product>(_dataProvider); } }
        public IQueryable<OrderItem> OrderItems { get { return AsyncRemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
