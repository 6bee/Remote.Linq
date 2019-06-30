// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Newtonsoft.Json;
    using Remote.Linq;
    using System;
    using System.Linq;
    using System.Net.Http.Formatting;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using System.Web.Http.Validation;

    public class WebApiServer : IDisposable
    {
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
                throw new Exception("Server has been opened already.");
            }

            // load common assembly into app domain
            var asm = typeof(Common.Model.ProductCategory).Assembly;

            var config = new HttpSelfHostConfiguration($"http://localhost:{_port}");

            config.Services.Replace(typeof(IBodyModelValidator), new CustomBodyModelValidator());

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().Single();
            //jsonFormatter.SerializerSettings = jsonFormatter.SerializerSettings.ConfigureRemoteLinq();
            //jsonFormatter.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            jsonFormatter.SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }.ConfigureRemoteLinq();

            config.Routes.MapHttpRoute("API Default", "api/{controller}");

            config.MaxReceivedMessageSize = 10 * 1024 * 1024;

            _server = new HttpSelfHostServer(config);

            _server.OpenAsync().Wait();
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }
    }
}