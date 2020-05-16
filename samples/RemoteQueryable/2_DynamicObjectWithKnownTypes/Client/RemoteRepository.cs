// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;

    public class RemoteRepository : IDisposable
    {
        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

        private readonly IDynamicObjectMapper _mapper;

        private readonly ChannelFactory<IQueryService> _channelFactory;

        public RemoteRepository(string uri)
        {
            _mapper = new DynamicObjectMapper(isKnownTypeProvider: new IsKnownTypeProvider());

            NetNamedPipeBinding binding = new NetNamedPipeBinding()
            {
                CloseTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(10),
                MaxReceivedMessageSize = 640000L,
            };

            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);

            _dataProvider = expression =>
                {
                    IQueryService channel = null;
                    try
                    {
                        channel = _channelFactory.CreateChannel();

                        IEnumerable<DynamicObject> result = channel.ExecuteQuery(expression);
                        return result;
                    }
                    finally
                    {
                        ICommunicationObject communicationObject = channel as ICommunicationObject;
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

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider, mapper: _mapper);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider, mapper: _mapper);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider, mapper: _mapper);

        public void Dispose() => ((IDisposable)_channelFactory).Dispose();
    }
}
