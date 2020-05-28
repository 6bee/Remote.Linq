// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using System;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using System.Web.Http.Validation;

    public sealed class WebApiServer : IDisposable
    {
        private sealed class CustomBodyModelValidator : DefaultBodyModelValidator
        {
            public override bool ShouldValidateType(Type type)
                => !typeof(Common.Model.Query).IsAssignableFrom(type)
                && base.ShouldValidateType(type);
        }

        private readonly int _port;
        private HttpSelfHostServer _server;

        public WebApiServer(int port)
        {
            _port = port;
        }

        public void Open()
        {
            if (_server != null)
            {
                throw new InvalidOperationException("Server has been opened already.");
            }

            // ensure common assembly is loaded
            _ = typeof(Common.Model.Query).Assembly;

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration($"http://localhost:{_port}");

            config.Services.Replace(typeof(IBodyModelValidator), new CustomBodyModelValidator());

            config.Routes.MapHttpRoute("API Default", "api/{controller}");

            config.MaxReceivedMessageSize = 10 * 1024 * 1024;

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();
        }

        public void Dispose() => _server?.Dispose();
    }
}
