// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using BlazorApp.Client.Services;
using BlazorApp.Components;
using BlazorApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddControllers() // add WebAPI controllers
    .AddJsonOptions(options => options.JsonSerializerOptions.ConfigureRemoteLinq()); // add System.Text.Json.JsonSerializerOptions for remote.linq

builder.Services
    .AddScoped<IDataProvider, DataProvider>(_ => new(new("https://services.odata.org")))
    .AddHttpContextAccessor();

builder.Services.AddHttpClient<IClientRepository, ClientRepository>(ConfigureHttpClient)
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5) })
    .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

var app = builder.Build();

// configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);

    // default HSTS value is 30 days. for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

app.MapControllers();

app.Run();

static void ConfigureHttpClient(IServiceProvider serviceProvider, HttpClient httpClient)
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext!.Request;
    var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
    httpClient.BaseAddress = new Uri(baseUrl);
}