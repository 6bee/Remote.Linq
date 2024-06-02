// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Controllers;

using Aqua.Dynamic;
using BlazorApp.Model;
using BlazorApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IDataProvider _dataProvider;

    public DataController(IDataProvider dataProvider)
        => _dataProvider = dataProvider;

    [Route("query")]
    public DynamicObject Query([FromBody] Query query)
        => _dataProvider.Execute(query.Expression);
}