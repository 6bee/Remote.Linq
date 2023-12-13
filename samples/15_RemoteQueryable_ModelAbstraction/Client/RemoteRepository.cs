// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Aqua.Dynamic;
    using Common.Model;
    using Common.ServiceContracts;
    using Remote.Linq;
    using Remote.Linq.Expressions;
    using System;
    using System.Linq;
    using System.ServiceModel;

    public sealed class RemoteRepository : IDisposable
    {
        private readonly ChannelFactory<IQueryService> _channelFactory;
        private readonly Func<Expression, DynamicObject> _dataProvider;

        public RemoteRepository(string uri)
        {
            var binding = new NetNamedPipeBinding() { MaxReceivedMessageSize = 10240 }.WithDebugSetting();
            _channelFactory = new ChannelFactory<IQueryService>(binding, uri);
            _dataProvider = expression =>
                {
                    using var proxy = _channelFactory.CreateServiceProxy();

                    var result = proxy.Service.ExecuteQuery(expression);
                    return result;
                };
        }

        public IQueryable<IEntity> Entities => RemoteQueryable.Factory.CreateQueryable<IEntity>(_dataProvider);

        public void Dispose() => _channelFactory.Close();
    }
}