// Copyright (c) Christof Senn. All rights reserved. 

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Remote.Linq;

namespace Client
{
    public sealed class ServiceProxy<T> : IDisposable
    {
        private readonly ChannelFactory<T> _channelFactory;
        private readonly T _channel;

        public ServiceProxy(string endpointConfigurationName)
        {
            _channelFactory = new ChannelFactory<T>(endpointConfigurationName);
            _channel = _channelFactory.CreateChannel();
        }

        public T Channel { get { return _channel; } }

        public Query<Entity> CreateQuery<Entity>(Func<T, Func<Query<Entity>, IEnumerable<Entity>>> serviceMethod)
        {
            return new Query<Entity>(serviceMethod(Channel));
        }

        public void Dispose()
        {
            try
            {
                ((IClientChannel)_channel).Close();
            }
            catch { }
            try
            {
                _channelFactory.Close();
            }
            catch { }
        }
    }
}
