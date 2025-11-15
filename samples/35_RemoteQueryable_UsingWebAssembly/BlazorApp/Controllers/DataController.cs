// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace BlazorApp.Controllers;

using Aqua.Dynamic;
using BlazorApp.Model;
using BlazorApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DataController(IDataProvider dataProvider) : ControllerBase
{
    [Route("query")]
    public DynamicObject Query([FromBody] Query query)
        => dataProvider.Execute(query.Expression);
}