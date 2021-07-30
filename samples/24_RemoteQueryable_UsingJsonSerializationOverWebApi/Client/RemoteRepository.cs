// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.Model;
    using Newtonsoft.Json;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using static CommonHelper;

    public class RemoteRepository : IRemoteRepository
    {
        private readonly JsonMediaTypeFormatter _formatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings().ConfigureRemoteLinq(),
        };

        private readonly HttpClient _httpClient;
        private readonly Func<Expression, ValueTask<DynamicObject>> _dataProvider;

        public RemoteRepository(string server, int port)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"http://{server}:{port}/") };
            _dataProvider = async expression =>
            {
                try
                {
                    var query = new Query { Expression = expression };
                    var response = await _httpClient.PostAsync("/api/query", query, _formatter).ConfigureAwait(false);

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        byte[] errorMessageData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                        string errorMessage = Encoding.UTF8.GetString(errorMessageData);
                        throw new RemoteLinqException(errorMessage);
                    }

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsAsync<DynamicObject>(new[] { _formatter }).ConfigureAwait(false);
                    return result;
                }
                catch (SocketException ex)
                {
                    PrintError(ex);
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncQueryable<OrderItem>(_dataProvider);

        public IQueryable<ProductGroup> ProductGroups => throw new NotImplementedException();

        public void Dispose() => _httpClient.Dispose();
    }
}
