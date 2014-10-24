// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Common;
using Common.Model;
using Remote.Linq;
using Remote.Linq.Dynamic;
using Remote.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Client
{
    public class RemoteRepository
    {
        private readonly Func<Expression, IEnumerable<DynamicObject>> _dataProvider;

        public RemoteRepository(string server, int port)
        {
            _dataProvider = expression =>
            {
                try
                {
                    IEnumerable<DynamicObject> result;

                    using (var client = new TcpClient(server, port))
                    {
                        using (var stream = client.GetStream())
                        {
                            stream.Write(expression);

                            result = stream.Read<IEnumerable<DynamicObject>>();

                            stream.Close();
                        }

                        client.Close();
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

        public IQueryable<ProductCategory> ProductCategories { get { return RemoteQueryable.Create<ProductCategory>(_dataProvider); } }
        public IQueryable<Product> Products { get { return RemoteQueryable.Create<Product>(_dataProvider); } }
        public IQueryable<OrderItem> OrderItems { get { return RemoteQueryable.Create<OrderItem>(_dataProvider); } }
    }
}
