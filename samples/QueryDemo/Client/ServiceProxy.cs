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

        public ServiceProxy(string endpointConfigurationName)
        {
            _channelFactory = new ChannelFactory<T>(endpointConfigurationName);
            Channel = _channelFactory.CreateChannel();
        }

        public T Channel { get; }

        public IQuery<Entity> CreateQuery<Entity>(Func<T, Func<Query<Entity>, IEnumerable<Entity>>> serviceMethod)
            => new Query<Entity>(serviceMethod(Channel));

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
