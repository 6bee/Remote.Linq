// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Common;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Dynamic;
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

                        var compressedData = channel.ExecuteQuery(expression);
                        var result = new CompressionHelper().Decompress(compressedData);
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

        public IQueryable<ProductCategory> ProductCategories { get { return RemoteQueryable.Create<ProductCategory>(_dataProvider); } }

        public IQueryable<Product> Products { get { return RemoteQueryable.Create<Product>(_dataProvider); } }

        public IQueryable<OrderItem> OrderItems { get { return RemoteQueryable.Create<OrderItem>(_dataProvider); } }

        public void Dispose()
        {
            ((IDisposable)_channelFactory).Dispose();
        }
    }
}
