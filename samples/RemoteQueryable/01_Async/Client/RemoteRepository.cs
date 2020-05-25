// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;

    public class RemoteRepository : IDisposable
    {
        private readonly ChannelFactory<IQueryService> _channelFactory;

        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _asyncDataProvider;

        public RemoteRepository(string uri)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding()
            {
                CloseTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                SendTimeout = TimeSpan.FromMinutes(10),
                MaxReceivedMessageSize = 640000L,
            };

            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);

            _asyncDataProvider = async expression =>
                {
                    IQueryService channel = null;
                    try
                    {
                        channel = _channelFactory.CreateChannel();

                        IEnumerable<DynamicObject> result = await channel.ExecuteQueryAsync(expression).ConfigureAwait(false);
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

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_asyncDataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_asyncDataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_asyncDataProvider);

        public void Dispose() => ((IDisposable)_channelFactory).Dispose();
    }
}
