// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Aqua.Dynamic;
using Common;
using Common.Model;
using Common.ServiceContracts;
using Remote.Linq;
using Remote.Linq.Expressions;
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

                byte[] compressedData = proxy.Service.ExecuteQuery(expression);
                DynamicObject result = new CompressionHelper().Decompress(compressedData);
                return result;
            };
    }

    public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

    public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateQueryable<ProductGroup>(_dataProvider);

    public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

    public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);

    public void Dispose() => _channelFactory.Close();
}