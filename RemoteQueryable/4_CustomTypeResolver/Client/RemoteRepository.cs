// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    public class RemoteRepository : IDisposable
    {
        private readonly ChannelFactory<IQueryService> _channelFactory;

        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

        public RemoteRepository(string uri)
        {
            var binding = new NetNamedPipeBinding()
            {
                CloseTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(10),
                MaxReceivedMessageSize = 640000L
            };
            
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);
            
            _dataProvider = expression =>
                {
                    IQueryService channel = null;
                    try
                    {
                        channel = _channelFactory.CreateChannel();

                        var result = channel.ExecuteQuery(expression);
                        return result;
                    }
                    finally
                    {
                        var communicationObject = channel as ICommunicationObject;
                        if (communicationObject != null)
                        {
                            if (communicationObject.State == CommunicationState.Faulted)
                            {
                                communicationObject.Abort();
                            }
                            else
                            {
                                communicationObject.Close();
                            }
                        }
                    }
                };
        }

        public IQueryable<ClientModel.ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ClientModel.ProductCategory>(_dataProvider);

        public IQueryable<ClientModel.Product> Products => RemoteQueryable.Factory.CreateQueryable<ClientModel.Product>(_dataProvider);
        
        public IQueryable<ClientModel.OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<ClientModel.OrderItem>(_dataProvider);

        public void Dispose()
        {
            ((IDisposable)_channelFactory).Dispose();
        }
    }
}
