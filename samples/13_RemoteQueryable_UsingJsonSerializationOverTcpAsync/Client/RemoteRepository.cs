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
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class RemoteRepository
    {
        private readonly Func<Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;

        public RemoteRepository(string server, int port)
        {
            _dataProvider = async expression =>
            {
                IEnumerable<DynamicObject> result;

                using (TcpClient client = new TcpClient(server, port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        await stream.WriteAsync(expression).ConfigureAwait(false);
                        await stream.FlushAsync().ConfigureAwait(false);

                        result = await stream.ReadAsync<IEnumerable<DynamicObject>>().ConfigureAwait(false);

                        stream.Close();
                    }

                    client.Close();
                }

                return result;
            };
        }

        public IQueryable<ProductCategory> ProductCategories => RemoteQueryable.Factory.CreateQueryable<ProductCategory>(_dataProvider);

        public IQueryable<Product> Products => RemoteQueryable.Factory.CreateQueryable<Product>(_dataProvider);

        public IQueryable<OrderItem> OrderItems => RemoteQueryable.Factory.CreateQueryable<OrderItem>(_dataProvider);
    }
}
