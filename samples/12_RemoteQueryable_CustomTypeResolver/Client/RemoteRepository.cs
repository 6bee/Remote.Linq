// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.ServiceModel;

    public sealed class RemoteRepository : IRemoteRepository
    {
        private readonly ChannelFactory<IQueryService> _channelFactory;

        private readonly Func<Expression, DynamicObject> _dataProvider;

        public RemoteRepository(string uri)
        {
            var binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);

            _dataProvider = expression =>
            {
                using var proxy = _channelFactory.CreateServiceProxy();

                DynamicObject result = proxy.Service.ExecuteQuery(expression);

                return result;
            };
        }

        public IQueryable<ClientModel.ProductCategory> ProductCategories => CreateQueryable<ClientModel.ProductCategory>();

        public IQueryable<ClientModel.ProductGroup> ProductGroups => CreateQueryable<ClientModel.ProductGroup>();

        public IQueryable<ClientModel.Product> Products => CreateQueryable<ClientModel.Product>();

        public IQueryable<ClientModel.OrderItem> OrderItems => CreateQueryable<ClientModel.OrderItem>();

        private IQueryable<T> CreateQueryable<T>()
            => RemoteQueryable.Factory.CreateQueryable<T>(
                _dataProvider,
                typeInfoProvider: new QueryTypeMapper(),
                mapper: new DynamicObjectMapper(typeResolver: new ResultTypeMapper()));

        public void Dispose() => _channelFactory.Close();
    }
}
