// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.Model;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class RemoteRepository
    {
        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;

        public RemoteRepository(string server, int port)
        {
            _dataProvider = async expression =>
            {
                try
                {
                    HttpClient client = new HttpClient
                    {
                        BaseAddress = new Uri($"http://{server}:{port}/"),
                    };

                    HttpResponseMessage response = await client.PostAsync("api/query", expression, new XmlMediaTypeFormatter());

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        byte[] errorMessageData = await response.Content.ReadAsByteArrayAsync();
                        string errorMessage = Encoding.UTF8.GetString(errorMessageData);
                        throw new Exception(errorMessage);
                    }

                    response.EnsureSuccessStatusCode();

                    IEnumerable<DynamicObject> result = await response.Content.ReadAsAsync<IEnumerable<DynamicObject>>();
                    return result;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Exception: {0}", ex);
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);
    }
}
