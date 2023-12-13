// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Remote.Linq.Text.Json;
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
                    .AddMvcCore(options => options.EnableEndpointRouting = false)
                    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureRemoteLinq())) // add system.text.json options for remote.linq
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