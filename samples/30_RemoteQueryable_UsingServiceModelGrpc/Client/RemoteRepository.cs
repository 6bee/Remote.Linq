// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Aqua.Dynamic;
using Common.Model;
using Common.ServiceContracts;
using Grpc.Core;
using Remote.Linq;
using Remote.Linq.Expressions;
using ServiceModel.Grpc.Client;

public sealed class RemoteRepository : IRemoteRepository
{
    private readonly Func<Expression, ValueTask<DynamicObject>> _dataProvider;

    public RemoteRepository(string host, int port)
    {
        var channel = new Channel(host, port, ChannelCredentials.Insecure);
        var proxy = ClientFactory.CreateClient<IQueryService>(channel);
        _dataProvider = async expression => await proxy.ExecuteQueryAsync(expression);
    }

    private IClientFactory ClientFactory { get; } = new ClientFactory();

    public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncQueryable<ProductCategory>(_dataProvider);

    public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncQueryable<Product>(_dataProvider);

    public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncQueryable<OrderItem>(_dataProvider);

    public IQueryable<ProductGroup> ProductGroups => RemoteQueryable.Factory.CreateAsyncQueryable<ProductGroup>(_dataProvider);

    public void Dispose()
    {
    }
}