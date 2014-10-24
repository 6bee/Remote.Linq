// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common;
using Common.Model;
using Remote.Linq;
using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Client
{
    public class RemoteRepository
    {
        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;

        public RemoteRepository(string url)
        {
            _dataProvider = async expression =>
            {
                try
                {
                    IEnumerable<DynamicObject> result;

                    HttpContent request;
                    using (var client = new HttpClient())
                    {
                        using (var requestStream = new MemoryStream())
                        using (var responseStream = new MemoryStream())
                        {
                            requestStream.Write(expression);
                            requestStream.Position = 0;

                            request = new StreamContent(requestStream);

                            using (var response = await client.PostAsync(url, request))
                            {
                                await response.Content.CopyToAsync(responseStream);
                                responseStream.Position = 0;

                                result = responseStream.Read<IEnumerable<DynamicObject>>();
                            }
                        }
                    }

                    return result;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException: {0}", ex);
                    throw;
                }
            };
        }

        public IQueryable<ProductCategory> ProductCategories { get { return AsyncRemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        public IQueryable<Product> Products { get { return AsyncRemoteQueryable.Create<Product>(_dataProvider); } }
        public IQueryable<OrderItem> OrderItems { get { return AsyncRemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
