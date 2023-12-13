// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client;

using Remote.Linq.SimpleQuery;
using System;
using System.Collections.Generic;
using System.ServiceModel;

public sealed class ServiceProxy<T> : IDisposable
{
    private readonly WcfServiceProxy<T> _proxy;

    public ServiceProxy(string uri)
    {
        var netTcpBinding = new NetTcpBinding { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
        _proxy = new WcfServiceProxy<T>(netTcpBinding, uri);
    }

    public T Service => _proxy.Service;

    public IQuery<TEntity> CreateQuery<TEntity>(Func<T, Func<Query<TEntity>, IEnumerable<TEntity>>> serviceMethod)
        => new Query<TEntity>(serviceMethod(Service));

    public void Dispose() => _proxy.Dispose();
}