// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Client
{
    using Remote.Linq;
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    public sealed class ServiceProxy<T> : IDisposable
    {
        private readonly ChannelFactory<T> _channelFactory;

        public ServiceProxy(string endpointConfigurationName)
        {
            _channelFactory = new ChannelFactory<T>(endpointConfigurationName);
            Channel = _channelFactory.CreateChannel();
        }

        public T Channel { get; }

        public IQuery<TEntity> CreateQuery<TEntity>(Func<T, Func<Query<TEntity>, IEnumerable<TEntity>>> serviceMethod)
            => new Query<TEntity>(serviceMethod(Channel));

        public void Dispose()
        {
            try
            {
                ((IClientChannel)Channel).Close();
            }
            catch
            {
            }

            try
            {
                _channelFactory.Close();
            }
            catch
            {
            }
        }
    }
}
