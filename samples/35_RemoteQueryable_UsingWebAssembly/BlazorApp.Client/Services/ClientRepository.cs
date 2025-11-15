// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Client.Services;

using Aqua.Dynamic;
using BlazorApp.Model;
using Remote.Linq;
using Remote.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

public sealed class ClientRepository : IClientRepository
{
    private static readonly JsonSerializerOptions? _jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }.ConfigureRemoteLinq();

    private readonly HttpClient _httpClient;
    private readonly Func<Expression, CancellationToken, ValueTask<DynamicObject>> _dataProvider;

    public ClientRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _dataProvider = async (expression, cancellationToken) =>
        {
            var query = new Query(expression);
            var response = await _httpClient.PostAsJsonAsync("/api/data/query", query, _jsonSerializerOptions, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode is HttpStatusCode.InternalServerError)
            {
                byte[] errorMessageData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                string errorMessage = Encoding.UTF8.GetString(errorMessageData);
                throw new RemoteLinqException(errorMessage);
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DynamicObject>(_jsonSerializerOptions).ConfigureAwait(false);
            return result ?? throw new Exception("Received empty value from server");
        };
    }

    public IQueryable<Person> People => RemoteQueryable.Factory.CreateAsyncQueryable<Person>(_dataProvider);
}