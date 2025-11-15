// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Services;

using Aqua.Dynamic;
using BlazorApp.Model;
using Microsoft.OData.Client;
using Remote.Linq.ExpressionExecution;
using Remote.Linq.Expressions;

public class DataProvider : IDataProvider
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    ];

    private readonly DataServiceContext _odataService;

    public DataProvider(Uri serviceRoot)
        => _odataService = new DataServiceContext(new Uri(serviceRoot, "TripPinRESTierService"));

    public DynamicObject Execute(Expression expression)
    {
        return expression.Execute(ResolveQueryableByType);

        IQueryable ResolveQueryableByType(Type type)
        {
            if (type == typeof(Person))
            {
                return _odataService.CreateQuery<Person>("People");
            }

            throw new NotSupportedException($"Type {type} is not supported as aggregate root");
        }
    }
}