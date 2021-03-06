﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Remote.Linq;
    using System;
    using System.Threading;

    public sealed class WebApiServer : IDisposable
    {
        private readonly CancellationTokenSource _cancellation;
        private readonly IWebHost _webHost;

        public WebApiServer(int port)
        {
            _cancellation = new CancellationTokenSource();
            _webHost = new WebHostBuilder()
                .ConfigureServices(services => services
                    .AddMvcCore()
                    .AddJsonFormatters()
                    .AddJsonOptions(json => json.SerializerSettings.ConfigureRemoteLinq()))
                .Configure(application => application.UseMvc())
                .UseKestrel()
                .UseUrls($"http://+:{port}")
                .Build();
        }

        public void Open() => _webHost.RunAsync(_cancellation.Token);

        public void Dispose()
        {
            _cancellation.Cancel();
            _cancellation.Dispose();
        }
    }
}
