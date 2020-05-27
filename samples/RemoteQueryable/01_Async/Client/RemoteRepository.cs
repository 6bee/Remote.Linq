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

    public sealed class RemoteRepository : IRemoteRepository
    {
        private readonly ChannelFactory<IQueryService> _channelFactory;

        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _asyncDataProvider;

        public RemoteRepository(string uri)
        {
            var binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 640000L }.WithDebugSetting();
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);

            _asyncDataProvider = async expression =>
                {
                    using var proxy = _channelFactory.CreateServiceProxy();

                    IEnumerable<DynamicObject> result = await proxy.Service.ExecuteQueryAsync(expression).ConfigureAwait(false);
                    return result;
                };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_asyncDataProvider);

        public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ProductGroup>(_asyncDataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_asyncDataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_asyncDataProvider);

        public void Dispose() => _channelFactory.Close();
    }
}
