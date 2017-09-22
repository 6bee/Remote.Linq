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
                    using (var client = new HttpClient())
                    using (var requestStream = new MemoryStream())
                    using (var responseStream = new MemoryStream())
                    {
                        await requestStream.WriteAsync(expression);
                        requestStream.Position = 0;

                        var request = new StreamContent(requestStream);

                        using (var response = await client.PostAsync(url, request))
                        {
                            await response.Content.CopyToAsync(responseStream);
                            responseStream.Position = 0;

                            var result = await responseStream.ReadAsync<IEnumerable<DynamicObject>>();
                            return result;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: {0}", ex);
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateAsyncQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateAsyncQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateAsyncQueryable<OrderItem>(_dataProvider);
    }
}
