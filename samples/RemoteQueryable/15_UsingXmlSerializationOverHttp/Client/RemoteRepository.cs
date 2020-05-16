// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common;
    using Common.Model;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class RemoteRepository
    {
        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;

        public RemoteRepository(string url)
        {
            _dataProvider = async expression =>
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    using (MemoryStream requestStream = new MemoryStream())
                    using (MemoryStream responseStream = new MemoryStream())
                    {
                        await requestStream.WriteAsync(expression);
                        requestStream.Position = 0;

                        StreamContent request = new StreamContent(requestStream);

                        using (HttpResponseMessage response = await client.PostAsync(url, request))
                        {
                            await response.Content.CopyToAsync(responseStream);
                            responseStream.Position = 0;

                            IEnumerable<DynamicObject> result = await responseStream.ReadAsync<IEnumerable<DynamicObject>>();
                            return result;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"SocketException: {ex.Message}");
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);
    }
}
