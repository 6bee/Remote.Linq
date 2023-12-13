// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Aqua.Dynamic;
using Common;
using Remote.Linq;
using Remote.Linq.Expressions;
using System;
using System.Linq;
using System.Net.Sockets;

internal class RemoteRepository
{
    private readonly Func<Expression, DynamicObject> _dataProvider;

    public RemoteRepository(string ip, int port)
    {
        _dataProvider = expression =>
        {
            using TcpClient client = new TcpClient(ip, port);
            using NetworkStream stream = client.GetStream();
            stream.Write(expression);
            stream.Flush();

            DynamicObject result = stream.Read<DynamicObject>();
            stream.Close();
            client.Close();
            return result;
        };
    }

    public IQueryable<int> Queryable => RemoteQueryable.Factory.CreateQueryable<int>(_dataProvider);
}