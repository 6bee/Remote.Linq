// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using BlazorApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<IClientRepository, ClientRepository>(serviceProvider =>
{
    return new(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
});

await builder.Build().RunAsync();