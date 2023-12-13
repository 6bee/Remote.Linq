// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Common.Model;
using Remote.Linq;
using Remote.Linq.Async.Queryable;
using System.Linq;
using System.Threading.Tasks;

public partial class AsyncTcpRemoteRepository : IAsyncRemoteRepository
{
    private readonly AsyncTcpQueryClient _client;

    public AsyncTcpRemoteRepository(string server, int port)
    {
        _client = new AsyncTcpQueryClient(server, port);
    }

    public IAsyncQueryable<ProductCategory> ProductCategories => CreateAsyncQueryable<ProductCategory>();

    public IAsyncQueryable<ProductGroup> ProductGroups => CreateAsyncQueryable<ProductGroup>();

    public IAsyncQueryable<Product> Products => CreateAsyncQueryable<Product>();

    public IAsyncQueryable<OrderItem> OrderItems => CreateAsyncQueryable<OrderItem>();

    public ValueTask DisposeAsync() => _client.DisposeAsync();

    // NOTE: One of `asyncStreamProvider` or `asyncDataProvider` parameter may also be set to null,
    //       in which case all queries are always fetched via stream or via batch query accordingly.
    private IAsyncQueryable<T> CreateAsyncQueryable<T>() => RemoteQueryable.Factory.CreateAsyncQueryable<T>(_client.ExecuteAsyncStream, _client.ExecuteAsync);
}