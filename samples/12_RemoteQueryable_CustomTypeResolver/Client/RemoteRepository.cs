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
        private readonly IExpressionTranslatorContext _context;

        public RemoteRepository(string uri)
        {
            var binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);
            _context = new ExpressionTranslatorContext(new ResultTypeMapper(), new QueryTypeMapper());
            _dataProvider = expression =>
            {
                using var proxy = _channelFactory.CreateServiceProxy();

                DynamicObject result = proxy.Service.ExecuteQuery(expression);

                return result;
            };
        }

        public IQueryable<ClientModel.ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ClientModel.ProductCategory>(_dataProvider, _context);

        public IQueryable<ClientModel.ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ClientModel.ProductGroup>(_dataProvider, _context);

        public IQueryable<ClientModel.Product> Products => RemoteQueryable.Factory.CreateQueryable<ClientModel.Product>(_dataProvider, _context);

        public IQueryable<ClientModel.OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<ClientModel.OrderItem>(_dataProvider, _context);

        public void Dispose() => _channelFactory.Close();
    }
}
