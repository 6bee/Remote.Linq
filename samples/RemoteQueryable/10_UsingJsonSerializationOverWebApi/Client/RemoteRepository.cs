// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.Model;
    using Newtonsoft.Json;
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
                    var client = new HttpClient()
                    {
                        BaseAddress = new Uri(string.Format("http://{0}:{1}/", server, port))
                    };

                    var formatter = new JsonMediaTypeFormatter();
                    formatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;

                    var response = await client.PostAsync("api/query", new Common.Model.Query { Expression = expression }, formatter);

                    if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var errorMessageData = await response.Content.ReadAsByteArrayAsync();
                        var errorMessage = Encoding.UTF8.GetString(errorMessageData);
                        throw new Exception(errorMessage);
                    }

                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsAsync<IEnumerable<DynamicObject>>(new[] { formatter });
                    return result;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Exception: {0}", ex);
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories { get { return AsyncRemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        
        public IQueryable<Product> Products { get { return AsyncRemoteQueryable.Create<Product>(_dataProvider); } }
        
        public IQueryable<OrderItem> OrderItems { get { return AsyncRemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
