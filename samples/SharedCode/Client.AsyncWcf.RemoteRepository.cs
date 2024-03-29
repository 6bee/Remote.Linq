﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Aqua.Dynamic;
using Common.Model;
using Common.ServiceContracts;
using Remote.Linq;
using Remote.Linq.Expressions;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

public sealed class RemoteRepository : IRemoteRepository
{
    private readonly ChannelFactory<IQueryService> _channelFactory;
    private readonly Func<Expression, ValueTask<DynamicObject>> _asyncDataProvider;

    public RemoteRepository(string uri)
    {
        var binding = new NetNamedPipeBinding { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
        _channelFactory = new ChannelFactory<IQueryService>(binding, uri);
        _asyncDataProvider = async expression =>
            {
                using var proxy = _channelFactory.CreateServiceProxy();
                var result = await proxy.Service.ExecuteQueryAsync(expression).ConfigureAwait(false);
                return result;
            };
    }

    public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncQueryable<ProductCategory>(_asyncDataProvider);

    public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateAsyncQueryable<ProductGroup>(_asyncDataProvider);

    public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncQueryable<Product>(_asyncDataProvider);

    public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncQueryable<OrderItem>(_asyncDataProvider);

    public void Dispose() => _channelFactory.Close();
}