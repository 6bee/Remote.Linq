// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Server;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public sealed class WebApiServer : IDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHost _host;

    public WebApiServer(int port)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging
            .SetMinimumLevel(LogLevel.None);
        builder.Services
            .AddControllers()
            .AddApplicationPart(typeof(QueryController).Assembly)
            .AddJsonOptions(x => x.JsonSerializerOptions.ConfigureRemoteLinq());
        builder.WebHost
            .UseUrls($"http://localhost:{port}");

        var app = builder.Build();
        app.MapControllers();
        _host = app;
    }

    public void Open() => _host.RunAsync(_cancellation.Token);

    public void Dispose()
    {
        _cancellation.Cancel();
        _cancellation.Dispose();

        _host.Dispose();
    }
}